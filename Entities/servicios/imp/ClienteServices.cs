using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class ClienteServices
    {    

        public Cliente GetCliente(int id, NpgsqlConnection conex ){

            string commandText =  getSelect() + GetFromText()+ GetWhereText() + "AND CL.\"ID_"+ Cliente.TABLA + "\" = @id";
            using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                    Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", id);
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            return ReadCliente(reader);
                            
                        }
                }
                return null;
                }

        public List<Cliente> listarClientes(NpgsqlConnection conex )
    {
        
        string commandText = getSelect() +  GetFromText() + GetWhereText(); 

        List<Cliente> clientes = new List<Cliente>();
        using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
        {

            Console.WriteLine("Consulta: " + commandText);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    clientes.Add(ReadCliente(reader));

                }

        }
        return clientes;

       
    }

public List<CondicionFiscal> GetCondicionFiscal(NpgsqlConnection conex)
{
    var lista = new List<CondicionFiscal>();

    string query = "SELECT \"ID_CONDICION\", \"CODIGO\", \"DESCRIPCION\" FROM \"CONDICION_AFIP\" ORDER BY \"DESCRIPCION\"";

    using (var cmd = new NpgsqlCommand(query, conex))
    {
        if (conex.State != System.Data.ConnectionState.Open)
            conex.Open();

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var item = new CondicionFiscal
                {
                    Id = reader.GetInt32(0),
                    Codigo = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
                lista.Add(item);
            }
        }
    }

    return lista;
}


 public Cliente Crear(NpgsqlConnection conn, Cliente cliente)
    {
        string query = @"
            INSERT INTO ""CLIENTE"" 
            (""RAZON_SOCIAL"", ""TELEFONO"", ""CONTACTO"", ""DOMICILIO"", ""LOCALIDAD"", ""CUIT"", ""ID_CONDICION_AFIP"", ""PROVINCIA"", ""TRANSPORTE"")
            VALUES (@razonSocial, @telefono, @contacto, @domicilio, @localidad, @cuit, @idCondicionAfip, @provincia, @transporte)
            RETURNING ""ID_CLIENTE"";
        ";


        using var cmd = new NpgsqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@razonSocial", cliente.RazonSocial ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@telefono", cliente.Telefono ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@contacto", cliente.Contacto ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@domicilio", cliente.Domicilio ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@localidad", cliente.Localidad ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@cuit", cliente.Cuit ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@idCondicionAfip", cliente.CondicionFiscal.Id);
        cmd.Parameters.AddWithValue("@provincia", cliente.Provincia ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@transporte", cliente.Transporte ?? (object)DBNull.Value);

        cliente.Id = (int)cmd.ExecuteScalar();
        return cliente;
    }

public int Actualizar(NpgsqlConnection conn, Cliente cliente)
{
    string query = @"
        UPDATE ""CLIENTE"" SET
            ""RAZON_SOCIAL"" = @razonSocial,
            ""TELEFONO"" = @telefono,
            ""CONTACTO"" = @contacto,
            ""DOMICILIO"" = @domicilio,
            ""LOCALIDAD"" = @localidad,
            ""CUIT"" = @cuit,
            ""ID_CONDICION_AFIP"" = @idCondicionAfip,
            ""PROVINCIA"" = @provincia,
            ""TRANSPORTE"" = @transporte
        WHERE ""ID_CLIENTE"" = @idCliente;
    ";

    using var cmd = new NpgsqlCommand(query, conn);
    cmd.Parameters.AddWithValue("@idCliente", cliente.Id);
    cmd.Parameters.AddWithValue("@razonSocial", cliente.RazonSocial ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@telefono", cliente.Telefono ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@contacto", cliente.Contacto ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@domicilio", cliente.Domicilio ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@localidad", cliente.Localidad ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@cuit", cliente.Cuit ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@idCondicionAfip", cliente.CondicionFiscal.Id);
    cmd.Parameters.AddWithValue("@provincia", cliente.Provincia ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@transporte", cliente.Transporte ?? (object)DBNull.Value);

    int filasAfectadas = cmd.ExecuteNonQuery();
    return filasAfectadas;
}




 private static string getSelect()
        {
            return  $"SELECT CL.* ,CF.\"CODIGO\" AS CF_CODIGO, CF.\"DESCRIPCION\" AS CF_DESCRIPCION ";
            
        }
    private static string GetFromText()
    {
        return "FROM \"CLIENTE\" CL,\"CONDICION_AFIP\" CF ";
    }



    private static string GetWhereText()
    {
        return "WHERE CL.\"ID_CONDICION_AFIP\" = CF.\"ID_CONDICION\" ";
    }


    private static Cliente ReadCliente(NpgsqlDataReader reader)
        {
            int? id = reader["ID_" + Cliente.TABLA] as int?;
            string rs = reader["RAZON_SOCIAL"] as string;
            string tel = reader["TELEFONO"] as string;
            string contacto = reader["CONTACTO"] as string;
            string domicilio = reader["DOMICILIO"] as string;
            string localidad = reader["LOCALIDAD"] as string;
            string telefono = reader["CUIT"] as string;
            string provincia = reader["PROVINCIA"] as string;
            string transporte = reader["TRANSPORTE"] as string;



             int? cfId = reader["ID_CONDICION_AFIP"] as int?;
            string cfCodigo = reader["CF_CODIGO"] as string;
            string cfDescripcion = reader["CF_DESCRIPCION"] as string;
            CondicionFiscal cf = new CondicionFiscal
            {
                Id = cfId.Value,
                Codigo =    cfCodigo,
                Descripcion = cfDescripcion
            };     

            return new Cliente
            {
                Id = id.Value,
                RazonSocial = rs,
                Telefono = tel,
                Contacto = contacto,
                Domicilio = domicilio,
                Localidad = localidad,
                Cuit = telefono,
                CondicionFiscal = cf,
                Provincia = provincia,
                Transporte = transporte

            };

            
        }


    }

