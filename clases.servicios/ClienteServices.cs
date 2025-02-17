using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class ClienteServices
    {    

        public Cliente GetCliente(int id, NpgsqlConnection conex ){
            string commandText = $"SELECT * FROM \" "+ Cliente.TABLA + "\"WHERE \"ID_"+ Cliente.TABLA + "\" = @id";
                using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
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
        
        string commandText = getSelect() +  GetFromText();

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

        static string getSelect()
        {
            return  $"SELECT CL.* ,CF.\"CODIGO\" AS CF_CODIGO, CF.\"DESCRIPCION\" AS CF_DESCRIPCION ";
            
        }
    }

    private static string GetFromText()
    {
        return "FROM \"CLIENTE\" CL,\"CONDICION_AFIP\" CF ";
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
                CondicionFiscal = cf

            };

            
        }


    }

