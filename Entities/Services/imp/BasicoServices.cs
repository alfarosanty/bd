using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public abstract class BasicoServices
    {

        public List<Basico> Listar(NpgsqlConnection conex){
            string commandText = $"SELECT * FROM \" "+ getTabla();
            List<Basico> listar = new List<Basico>();
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex)){
                Console.WriteLine("Consulta: "+ commandText);
                using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                          listar.Add(readBasico(reader));      
                        }

            }
            return listar;
        }


        public Basico get(int id,NpgsqlConnection conex){
            string commandText = $"SELECT * FROM \" "+ getTabla()+ "\"WHERE \"ID_"+ Articulo.TABLA + "\" = @id";
            //List<Basico> listar = new List<Basico>();
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex)){
                Console.WriteLine("Consulta: "+ commandText);
                cmd.Parameters.AddWithValue("id", id);
                using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                          return readBasico(reader);      
                        }

            }
            return null;
        }

        public abstract Basico readBasico(NpgsqlDataReader reader);
        public abstract String getTabla();
        
        }


    


