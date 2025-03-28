

using Npgsql;

public class TallerServices
{
    public string getTabla()
    {
        return Taller.TABLA;
    }


public Taller GetTaller(int id, NpgsqlConnection conex ){

            string commandText =  getSelect() + GetFromText() + " WHERE F.\"ID_"+ Taller.TABLA + "\" = @id";
            using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                    Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", id);
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            return ReadTaller(reader);
                            
                        }
                }
                return null;
                }

    public List<Taller> listarTalleres(NpgsqlConnection conex)
    {
        
        string commandText = getSelect() +  GetFromText();

        List<Taller> talleres = new List<Taller>();
        using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
        {

            Console.WriteLine("Consulta: " + commandText);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    talleres.Add(ReadTaller(reader));

                }

        }
        return talleres;

       
    }


    private static Taller ReadTaller(NpgsqlDataReader reader)
        {
            int? id = reader["ID_" + Taller.TABLA] as int?;
            string rs = reader["RAZON_SOCIAL"] as string;
            string tel = reader["TELEFONO"] as string;
            string direccion = reader["DIRECCION"] as string;
            string provincia = reader["PROVINCIA"] as string;

            return new Taller
            {
                Id = id.Value,
                razonSocial = rs,
                telefono = tel,
                direccion = direccion,
                provincia = provincia,

            };

            
        }


    private static string getSelect()
    {
        return $"SELECT F.\"ID_FABRICANTE\", F.\"RAZON_SOCIAL\", F.\"TELEFONO\", F.\"PROVINCIA\", F.\"DIRECCION\"";
    }

    private static string GetFromText()
    {
        return "FROM \"FABRICANTE\" F";
    }

}