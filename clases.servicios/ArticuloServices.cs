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
                return "SELECT AR.*, " +
                    "MD.\"CODIGO\" AS MEDIDA_CODIGO, MD.\"DESCRIPCION\" AS MEDIDA_DESCRIPCION, " +
                    "FM.\"CODIGO\" AS FAMILIA_CODIGO, FM.\"DESCRIPCION\" AS FAMILIA_DESCRIPCION, " +
                    "CL.\"CODIGO\" AS COLOR_CODIGO, CL.\"DESCRIPCION\" AS COLOR_DESCRIPCION, " +
                    "AP.\"CODIGO\" AS ARTICULO_PRECIO_CODIGO, AP.\"DESCRIPCION\" AS ARTICULO_PRECIO_DESCRIPCION, " +
                    "AP.\"PRECIO1\", AP.\"PRECIO2\", AP.\"PRECIO3\" ";            }

            static string NewMethodDistintos()
            {
                return "SELECT DISTINCT ON (AR.\"ID_MEDIDA\", AR.\"ID_FAMILIA\") AR.*, " +
                    "MD.\"CODIGO\" AS MEDIDA_CODIGO, MD.\"DESCRIPCION\" AS MEDIDA_DESCRIPCION, " +
                    "FM.\"CODIGO\" AS FAMILIA_CODIGO, FM.\"DESCRIPCION\" AS FAMILIA_DESCRIPCION, " +
                    "CL.\"CODIGO\" AS COLOR_CODIGO, CL.\"DESCRIPCION\" AS COLOR_DESCRIPCION, " +
                    "AP.\"CODIGO\" AS ARTICULO_PRECIO_CODIGO, AP.\"DESCRIPCION\" AS ARTICULO_PRECIO_DESCRIPCION, " +
                    "AP.\"PRECIO1\", AP.\"PRECIO2\", AP.\"PRECIO3\" ";
                    }

 

        public Articulo GetArticulo(int id, NpgsqlConnection conex ){
            string commandText = $"SELECT * FROM \""+ Articulo.TABLA + "\" WHERE \"ID_"+ Articulo.TABLA + "\" = @id";

            string selectText = NewMethod();
            string fromText = "FROM \"ARTICULO\" AR,\"MEDIDA\" MD, \"FAMILIA\" FM, \"COLOR\" CL, \"ARTICULO_PRECIO\" AP ";
            string whereText = "WHERE AR.\"ID_MEDIDA\"= MD.\"ID_MEDIDA\" AND AR.\"ID_FAMILIA\"= FM.\"ID_FAMILIA\" AND";
            whereText += " AR.\"ID_COLOR\"= CL.\"ID_COLOR\" AND ";
            whereText += " AR.\"ID_ARTICULO_PRECIO\"= AP.\"ID_ARTICULO_PRECIO\" AND ";
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

    public List<Articulo> listarArticulos(NpgsqlConnection conex, bool distintos )
    {
        string selectText = distintos ? NewMethodDistintos() : NewMethod();

        string fromText = "FROM \"ARTICULO\" AR, \"MEDIDA\" MD, \"FAMILIA\" FM, \"COLOR\" CL, \"ARTICULO_PRECIO\" AP ";
        
        string whereText = "WHERE AR.\"ID_MEDIDA\" = MD.\"ID_MEDIDA\" " +
                        "AND AR.\"ID_FAMILIA\" = FM.\"ID_FAMILIA\" " +
                        "AND AR.\"ID_COLOR\" = CL.\"ID_COLOR\" " +
                        "AND AR.\"ID_ARTICULO_PRECIO\" = AP.\"ID_ARTICULO_PRECIO\" ";

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




    public List<Articulo> GetArticuloByFamiliaMedida(string familia, string medida, NpgsqlConnection conex ){
            string selectText = $"SELECT AR.* ,";  
            selectText += "MD.\"CODIGO\" AS MEDIDA_CODIGO, MD.\"DESCRIPCION\" AS MEDIDA_DESCRIPCION, ";
            selectText += "FM.\"CODIGO\" AS FAMILIA_CODIGO, FM.\"DESCRIPCION\" AS FAMILIA_DESCRIPCION, ";
            selectText += "CL.\"CODIGO\" AS COLOR_CODIGO, CL.\"DESCRIPCION\" AS COLOR_DESCRIPCION ";
            selectText += "AP.\"CODIGO\" AS ARTICULO_PRECIO_CODIGO, AP.\"DESCRIPCION\" AS ARTICULO_PRECIO_DESCRIPCION ";
            string fromText = "FROM \"ARTICULO\" AR,\"MEDIDA\" MD, \"FAMILIA\" FM, \"COLOR\" CL, \"ARTICULO_PRECIO\" AP ";
            string whereText = "WHERE AR.\"ID_MEDIDA\"= MD.\"ID_MEDIDA\" AND AR.\"ID_FAMILIA\"= FM.\"ID_FAMILIA\" AND";
            whereText += " AR.\"ID_COLOR\"= CL.\"ID_COLOR\" ";
            whereText += " AR.\"ID_ARTICULO_PRECIO\"= AP.\"ID_ARTICULO_PRECIO\" ";

            if(familia !=null )
               whereText +=  "AND FM.\"CODIGO\"=@id_familia ";
            
            if(medida!=null)
                whereText +="AND MD.\"CODIGO\" =@id_medida";
            string commandText = selectText + fromText + whereText;
            Console.WriteLine("Consulta: "+ commandText + "MEDIDA= " + medida + " FAMILIA= "+ familia);
            List<Articulo> articulos = new List<Articulo>();
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
            {
                 if(medida!=null)
                    cmd.Parameters.AddWithValue("id_medida", medida);
                if(familia !=null )
                    cmd.Parameters.AddWithValue("id_familia", familia);
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


            public List<ArticuloPrecio> GetArticuloPrecio(NpgsqlConnection conex){
    string query = "SELECT \"ID_ARTICULO_PRECIO\", \"CODIGO\", \"DESCRIPCION\", \"PRECIO1\", \"PRECIO2\", \"PRECIO3\" FROM \"ARTICULO_PRECIO\"";
    
    List<ArticuloPrecio> articulosPrecios = new List<ArticuloPrecio>();
    
    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conex))
    {
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                ArticuloPrecio precio = new ArticuloPrecio
                {
                    articuloPrecioId = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO_PRECIO")),
                    Codigo = reader.GetString(reader.GetOrdinal("CODIGO")),
                    Descripcion = reader.GetString(reader.GetOrdinal("DESCRIPCION")),
                    Precio1 = reader.IsDBNull(reader.GetOrdinal("PRECIO1")) ? 0m : reader.GetDecimal(reader.GetOrdinal("PRECIO1")),
                    Precio2 = reader.IsDBNull(reader.GetOrdinal("PRECIO2")) ? 0m : reader.GetDecimal(reader.GetOrdinal("PRECIO2")),
                    Precio3 = reader.IsDBNull(reader.GetOrdinal("PRECIO3")) ? 0m : reader.GetDecimal(reader.GetOrdinal("PRECIO3"))
                };
                articulosPrecios.Add(precio);
            }
        }
    }

    return articulosPrecios;
}


        private static Articulo ReadArticulo(NpgsqlDataReader reader)
        {
            int? id = reader["ID_" + Articulo.TABLA] as int?;
            string name = reader["CODIGO"] as string;
            string minPlayers = reader["DESCRIPCION"] as string;
            int? idFabricante = reader["ID_FABRICANTE"] as int?;
            
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

            int? articuloPrecioId = reader["ID_" + ArticuloPrecio.TABLA] as int?;            
            string codigo = reader["CODIGO"] as string;   
            string descripcion = reader["DESCRIPCION"] as string;
            int precio1Index = reader.GetOrdinal("PRECIO1");
            int precio1 = reader.IsDBNull(precio1Index) ? 0 : reader.GetInt32(precio1Index);
            int precio2Index = reader.GetOrdinal("PRECIO2");
            int precio2 = reader.IsDBNull(precio2Index) ? 0 : reader.GetInt32(precio2Index);
            int precio3Index = reader.GetOrdinal("PRECIO3");
            int precio3 = reader.IsDBNull(precio3Index) ? 0 : reader.GetInt32(precio3Index);
   
            ArticuloPrecio articuloPrecio = new ArticuloPrecio
            {
                articuloPrecioId = articuloPrecioId.Value,
                Codigo =    codigo,
                Descripcion = descripcion,
                Precio1 = precio1,
                Precio2 = precio2,
                Precio3 = precio3,
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
            articulo.articuloPrecio = articuloPrecio;
            articulo.IdFabricante = idFabricante.Value;
            return articulo;
        }


    }

