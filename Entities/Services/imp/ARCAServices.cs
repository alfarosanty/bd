using System.Security;
using BlumeAPI.Entities;
using BlumeAPI.Services;

public class ARCAService : IARCAService
{

    private readonly IUnitOfWork _iUnitOfWork;
    public ARCAService(IUnitOfWork iUnitOfWork)
    {
        _iUnitOfWork = iUnitOfWork;
    }

    public async Task<LoginTicketResponseData> AutenticacionAsync()
    {
        Console.WriteLine("Iniciando proceso de autenticación...");

        try
        {
            // 1️⃣ Buscamos el token actual usando tu nuevo repositorio
            var auth = await _iUnitOfWork.Arca.ObtenerAutenticacionAsync();

            if (auth != null && auth.Expiracion.ToUniversalTime() > DateTime.UtcNow)
            {
                Console.WriteLine("Token válido encontrado en base de datos.");
                return new LoginTicketResponseData
                {
                    Token = auth.Token,
                    Sign = auth.Firma,
                    ExpirationTime = auth.Expiracion,
                    GenerationTime = DateTime.Now, // Esto es informativo
                    UniqueId = auth.UniqueId
                };
            }

            Console.WriteLine("Token expirado o inexistente. Solicitando nuevo...");

            // 2️⃣ Obtenemos configuración desde la BD
            var config = await _iUnitOfWork.Arca.ObtenerConfiguracionAsync() 
                        ?? throw new Exception("No se encontró configuración en DATOS_AFIP");

            // 3️⃣ Construimos SecureString
            SecureString passwordSecure = new SecureString();
            foreach (char c in config.Contrasena) passwordSecure.AppendChar(c);
            passwordSecure.MakeReadOnly();

            // 4️⃣ Solicitamos nuevo LoginTicket a ARCA
            var loginTicket = new LoginTicket();
            var loginResponse = await loginTicket.ObtenerLoginTicketResponse(
                config.Certificado,
                passwordSecure,
                config.Servicio,
                config.UrlWsaa,
                true // verbose
            );

            Console.WriteLine("LoginTicket generado exitosamente.");

            // 5️⃣ Guardamos el nuevo token en la BD usando el Repositorio
            var nuevaAuth = new DatosAutenticacion
            {
                UniqueId = loginResponse.UniqueId,
                Token = loginResponse.Token,
                Firma = loginResponse.Sign,
                Expiracion = loginResponse.ExpirationTime
            };

            await _iUnitOfWork.Arca.GuardarAutenticacionAsync(nuevaAuth);
            await _iUnitOfWork.SaveChangesAsync(); // Persistimos los cambios

            return loginResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR crítico en autenticación: {ex.Message}");
            throw;
        }
    }

}

