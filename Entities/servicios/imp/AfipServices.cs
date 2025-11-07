using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography.X509Certificates;

public class AfipServices
{

public async Task<LoginTicketResponseData> AutenticacionAsync(bool verbose, NpgsqlConnection con)
{

    Console.WriteLine("Estoy en la fncion del login");
    if (con == null)
        throw new ArgumentNullException(nameof(con), "La conexión no puede ser null");

    try
    {
        // 1️⃣ Revisamos si hay token válido en la BD
        DateTime horaExpiracion = DateTime.MinValue;
        string token = null!;
        string firma = null!;

        using (var cmd = new NpgsqlCommand(
            @"SELECT ""EXPIRACION"", ""TOKEN"", ""FIRMA"" FROM ""DATOS_AUTENTICACION"" LIMIT 1", con))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                // Obtén el índice de la columna una sola vez
            int indexExpiracion = reader.GetOrdinal("EXPIRACION");

// Lee el valor de manera segura
                if (reader.IsDBNull(indexExpiracion))
                {
                    horaExpiracion = DateTime.MinValue; // o cualquier valor por defecto
                }
                else
                {
                    // PostgreSQL timestamp without time zone → DateTimeKind.Unspecified
                    var dbValue = reader.GetFieldValue<DateTime>(indexExpiracion);

                    // Si querés tratarlo como UTC o Local, especifica el Kind
                    horaExpiracion = DateTime.SpecifyKind(dbValue, DateTimeKind.Utc); // o Local
                    Console.WriteLine("este es la fecha que me trae: " + horaExpiracion);
                }

                token = reader.IsDBNull(reader.GetOrdinal("TOKEN")) ? null! : reader.GetString(reader.GetOrdinal("TOKEN"));
                firma = reader.IsDBNull(reader.GetOrdinal("FIRMA")) ? null! : reader.GetString(reader.GetOrdinal("FIRMA"));
            }
        }

        //* 2️⃣ Si token aún es válido, lo devolvemos
        if (horaExpiracion > DateTime.UtcNow)
        {
            if (verbose) Console.WriteLine("Token válido encontrado en BD");
            return new LoginTicketResponseData
            {
                Token = token,
                Sign = firma,
                ExpirationTime = horaExpiracion,
                GenerationTime = DateTime.Now,
                UniqueId = 0
            };
        }

        if (verbose) Console.WriteLine("Token expirado o no encontrado, solicitando nuevo...");
//*/
        // 3️⃣ Leemos certificado, contraseña, servicio y URLWSAA
        byte[] certBytes;
        string contrasena;
        string servicio;
        string urlWsaa;

        using (var cmd = new NpgsqlCommand(
            @"SELECT ""CERTIFICADO"", ""CONTRASENA"", ""SERVICIO"", ""URLWSAA"" 
              FROM ""DATOS_AFIP"" LIMIT 1", con))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (!await reader.ReadAsync())
                throw new Exception("No se encontró configuración en DATOS_AFIP");

            certBytes = (byte[])reader["CERTIFICADO"];
            contrasena = reader.IsDBNull(reader.GetOrdinal("CONTRASENA"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("CONTRASENA"));
            servicio = reader.IsDBNull(reader.GetOrdinal("SERVICIO"))
                ? throw new Exception("Falta SERVICIO en DATOS_AFIP")
                : reader.GetString(reader.GetOrdinal("SERVICIO"));
            urlWsaa = reader.IsDBNull(reader.GetOrdinal("URLWSAA"))
                ? throw new Exception("Falta URLWSAA en DATOS_AFIP")
                : reader.GetString(reader.GetOrdinal("URLWSAA"));
        }

        // 4️⃣ Construimos SecureString a partir de la contraseña
        SecureString passwordSecure = new SecureString();
        foreach (char c in contrasena) passwordSecure.AppendChar(c);
        passwordSecure.MakeReadOnly();

        // 5️⃣ Solicitamos nuevo LoginTicket
        var loginTicket = new LoginTicket();
        var loginResponse = await loginTicket.ObtenerLoginTicketResponse(
            certBytes,
            passwordSecure,
            servicio,
            urlWsaa,
            verbose
        );

        if (verbose) Console.WriteLine("LoginTicket generado correctamente.");

        // 6️⃣ Guardamos token nuevo en la BD
        using (var cmdUpdate = new NpgsqlCommand(
            @"UPDATE ""DATOS_AUTENTICACION"" 
              SET ""TOKEN"" = @token, ""FIRMA"" = @firma, ""EXPIRACION"" = @expiracion", con))
        {
            cmdUpdate.Parameters.AddWithValue("token", loginResponse.Token);
            cmdUpdate.Parameters.AddWithValue("firma", loginResponse.Sign);
            cmdUpdate.Parameters.AddWithValue("expiracion", loginResponse.ExpirationTime);
            await cmdUpdate.ExecuteNonQueryAsync();
        }

        return loginResponse;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        throw;
    }
}


}

