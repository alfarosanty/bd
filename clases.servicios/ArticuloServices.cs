using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class ArticuloServices
    {   

        static string NewMethod()
        {
            string selectText = $"SELECT AR.* ,";
            selectText += "MD.\"CODIGO\" AS MEDIDA_CODIGO, MD.\"DESCRIPCION\" AS MEDIDA_DESCRIPCION, ";
            selectText += "FM.\"CODIGO\" AS FAMILIA_CODIGO, FM.\"DESCRIPCION\" AS FAMILIA_DESCRIPCION, ";
            selectText += "CL.\"CODIGO\" AS COLOR_CODIGO, CL.\"DESCRIPCION\" AS COLOR_DESCRIPCION ";
            return selectText;
        } 

        public Articulo GetArticulo(int id, NpgsqlConnection conex ){
            string commandText = $"SELECT * FROM \""+ Articulo.TABLA + "\" WHERE \"ID_"+ Articulo.TABLA + "\" = @id";

            string selectText = NewMethod();
            string fromText = "FROM \"ARTICULO\" AR,\"MEDIDA\" MD, \"FAMILIA\" FM, \"COLOR\" CL ";
            string whereText = "WHERE AR.\"ID_MEDIDA\"= MD.\"ID_MEDIDA\" AND AR.\"ID_FAMILIA\"= FM.\"ID_FAMILIA\" AND";
            whereText += " AR.\"ID_COLOR\"= CL.\"ID_COLOR\" AND ";
            whereText += "AR.\"ID_"+ Articulo.TABLA + "\" = @id";
            commandText = selectText + fromText + whereText;     
            
                using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
                {
                    Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", id);
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            Articulo game = ReadArticulo(reader);
                            return game;
                        }
                }
                return null;
                }

                 public List<Articulo> listarArticulos(NpgsqlConnection conex )
    {
        string selectText = NewMethod();
        string fromText = "FROM \"ARTICULO\" AR,\"MEDIDA\" MD, \"FAMILIA\" FM, \"COLOR\" CL ";
        string whereText = "WHERE AR.\"ID_MEDIDA\"= MD.\"ID_MEDIDA\" AND AR.\"ID_FAMILIA\"= FM.\"ID_FAMILIA\" AND";
        whereText += " AR.\"ID_COLOR\"= CL.\"ID_COLOR\"";
        string commandText = selectText + fromText + whereText;

        List<Articulo> articulos = new List<Articulo>();
        using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
        {

            Console.WriteLine("Consulta: " + commandText);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    articulos.Add(ReadArticulo(reader));

                }

        }
        return articulos;

        
    }




    public List<Articulo> GetArticuloByFamiliaMedida(Familia familia, Medida medida, NpgsqlConnection conex ){
            string selectText = $"SELECT AR.* ,";  
            selectText += "MD.\"CODIGO\" AS MEDIDA_CODIGO, MD.\"DESCRIPCION\" AS MEDIDA_DESCRIPCION, ";
            selectText += "FM.\"CODIGO\" AS FAMILIA_CODIGO, FM.\"DESCRIPCION\" AS FAMILIA_DESCRIPCION, ";
            selectText += "CL.\"CODIGO\" AS COLOR_CODIGO, CL.\"DESCRIPCION\" AS COLOR_DESCRIPCION ";
            string fromText = "FROM \"ARTICULO\" AR,\"MEDIDA\" MD, \"FAMILIA\" FM, \"COLOR\" CL ";
            string whereText = "WHERE AR.\"ID_MEDIDA\"= MD.\"ID_MEDIDA\" AND AR.\"ID_FAMILIA\"= FM.\"ID_FAMILIA\" AND";
            whereText += " AR.\"ID_COLOR\"= CL.\"ID_COLOR\" AND FM.\"CODIGO\"=@id_familia AND ";
            whereText +="MD.\"CODIGO\" =@id_medida";
            string commandText = selectText + fromText + whereText;
            Console.WriteLine("Consulta: "+ commandText + "MEDIDA= " + medida.Codigo + " FAMILIA= "+ familia.Codigo);
            List<Articulo> articulos = new List<Articulo>();
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
            {
                cmd.Parameters.AddWithValue("id_medida", medida.Codigo.ToString());
                cmd.Parameters.AddWithValue("id_familia", familia.Codigo.ToString());
                using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                {                    
                    while (reader.Read())
                        {   
                        articulos.Add( ReadArticulo(reader));
                        }
                }
            }                
            return articulos;
                
            }


        private static Articulo ReadArticulo(NpgsqlDataReader reader)
        {
            int? id = reader["ID_" + Articulo.TABLA] as int?;
            string name = reader["CODIGO"] as string;
            string minPlayers = reader["DESCRIPCION"] as string;

            
            int? medidaId = reader["ID_" + Medida.TABLA] as int?;            
            string meidadCodigo = reader["MEDIDA_CODIGO"] as string;   
            string meidadDescripcion = reader["MEDIDA_DESCRIPCION"] as string;   
            Medida medida =new Medida
            {
                Id = medidaId.Value,
                Codigo =    meidadCodigo,
                Descripcion = meidadDescripcion
            };     

            int? colorId = reader["ID_" + Color.TABLA] as int?;            
            string colorCodigo = reader["COLOR_CODIGO"] as string;   
            string colorDescripcion = reader["COLOR_DESCRIPCION"] as string;   
            Color color =new Color
            {
                Id = colorId.Value,
                Codigo =    colorCodigo,
                Descripcion = colorDescripcion
            };     

            int? familiaId = reader["ID_" + Familia.TABLA] as int?;            
            string familiaCodigo = reader["FAMILIA_CODIGO"] as string;   
            string familiaDescripcion = reader["FAMILIA_DESCRIPCION"] as string;   
            Familia familia = new Familia
            {
                Id = familiaId.Value,
                Codigo =    familiaCodigo,
                Descripcion = familiaDescripcion
            }; 

            Console.WriteLine("a cargar el articulo");

            Articulo  articulo = new Articulo
            {
                Id = id.Value,
                Codigo = name,
                Descripcion = minPlayers
            };

            articulo.Medida = medida;
            articulo.Color = color;
            articulo.Familia = familia;
            return articulo;
        }


    }

