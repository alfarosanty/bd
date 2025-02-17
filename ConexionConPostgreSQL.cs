using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class CConexion
    {
        NpgsqlConnection conex = new NpgsqlConnection();
        static String servidor="localhost";
        static String bd="BD";
        static String usuario="postgres";
        static String password="admin";
        static String puerto = "5432";

        String cadenaConexion = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + password + ";" + "database=" + bd + ";";

        public NpgsqlConnection establecerConexion() {

            try {
                conex.ConnectionString = cadenaConexion;
                conex.Open();
                Console.WriteLine("Se conectó correctamente a la Base de datos");
            }

            catch (NpgsqlException e) {
                Console.WriteLine("No se pudo conectar a la base de datos de PostgreSQL, error: "+ e.ToString());
            
            }

            return conex;
        }
        public void cerrarConexion(NpgsqlConnection conex ){
            try {
                conex.Close();
                Console.WriteLine("Se desconecto  correctamente a la Base de datos");
            }catch (NpgsqlException e) {
                Console.WriteLine("No se pudo desconectar a la base de datos de PostgreSQL, error: "+ e.ToString());
            
            }
        }

        public Color GetColor(int id, NpgsqlConnection conex ){
            string commandText = $"SELECT * FROM \"COLOR\"WHERE \"ID_COLOR\" = @id";
                using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
                {
                    Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", id);
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            Color game = ReadColor(reader);
                            return game;
                        }
                }
                return null;
                }

                 public List<Color> listarColores(NpgsqlConnection conex ){
                string commandText = $"SELECT * FROM \"COLOR\"";
                List<Color> colores = new List<Color>();
                using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
                    {
                
                        Console.WriteLine("Consulta: "+ commandText);
                        using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                colores.Add( ReadColor(reader));
                                
                            }
                    
                    }
                        return colores;
                }

        private static Color ReadColor(NpgsqlDataReader reader)
        {
            int? id = reader["ID_COLOR"] as int?;
            string name = reader["CODIGO"] as string;
            string minPlayers = reader["DESCRIPCION"] as string;
            

            Color color = new Color
            {
                Id = id.Value,
                Codigo = name,
                Descripcion = minPlayers
            };
            return color;
        }


    }