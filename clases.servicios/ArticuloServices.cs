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
                    "SFM.\"CODIGO\" AS SUBFAMILIA_CODIGO, SFM.\"DESCRIPCION\" AS SUBFAMILIA_DESCRIPCION, " +
                    "CL.\"CODIGO\" AS COLOR_CODIGO, CL.\"DESCRIPCION\" AS COLOR_DESCRIPCION, CL.\"HEXA\" AS COLOR_HEXA, " +
                    "AP.\"CODIGO\" AS ARTICULO_PRECIO_CODIGO, AP.\"DESCRIPCION\" AS ARTICULO_PRECIO_DESCRIPCION, " +
                    "AP.\"PRECIO1\", AP.\"PRECIO2\", AP.\"PRECIO3\" ";            }

            static string NewMethodDistintos()
            {
                return "SELECT DISTINCT ON (AR.\"ID_MEDIDA\", AR.\"ID_SUBFAMILIA\") AR.*, " +
                    "MD.\"CODIGO\" AS MEDIDA_CODIGO, MD.\"DESCRIPCION\" AS MEDIDA_DESCRIPCION, " +
                    "SFM.\"CODIGO\" AS SUBFAMILIA_CODIGO, SFM.\"DESCRIPCION\" AS SUBFAMILIA_DESCRIPCION, " +
                    "CL.\"CODIGO\" AS COLOR_CODIGO, CL.\"DESCRIPCION\" AS COLOR_DESCRIPCION, CL.\"HEXA\" AS COLOR_HEXA, " +
                    "AP.\"CODIGO\" AS ARTICULO_PRECIO_CODIGO, AP.\"DESCRIPCION\" AS ARTICULO_PRECIO_DESCRIPCION, " +
                    "AP.\"PRECIO1\", AP.\"PRECIO2\", AP.\"PRECIO3\" ";
                    }

 

        public Articulo GetArticulo(int id, NpgsqlConnection conex ){
            string commandText = $"SELECT * FROM \""+ Articulo.TABLA + "\" WHERE \"ID_"+ Articulo.TABLA + "\" = @id";

            string selectText = NewMethod();
            string fromText = "FROM \"ARTICULO\" AR,\"MEDIDA\" MD, \"SUBFAMILIA\" SFM, \"COLOR\" CL, \"ARTICULO_PRECIO\" AP ";
            string whereText = "WHERE AR.\"ID_MEDIDA\"= MD.\"ID_MEDIDA\" AND AR.\"ID_SUBFAMILIA\"= SFM.\"ID_SUBFAMILIA\" AND";
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

        string fromText = "FROM \"ARTICULO\" AR, \"MEDIDA\" MD, \"SUBFAMILIA\" SFM, \"COLOR\" CL, \"ARTICULO_PRECIO\" AP ";
        
        string whereText = "WHERE AR.\"ID_MEDIDA\" = MD.\"ID_MEDIDA\" " +
                        "AND AR.\"ID_SUBFAMILIA\" = SFM.\"ID_SUBFAMILIA\" " +
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



       public List<Articulo> GetArticulosByArticuloPrecioId(int articuloPrecioId, NpgsqlConnection conex)
{
    string select = NewMethod(); // Reutilizás el SELECT común
    string fromAndWhere = @"
                            FROM 
                                ""ARTICULO"" AR, 
                                ""MEDIDA"" MD, 
                                ""SUBFAMILIA"" SFM, 
                                ""COLOR"" CL, 
                                ""ARTICULO_PRECIO"" AP
                            WHERE 
                                AR.""ID_MEDIDA"" = MD.""ID_MEDIDA"" AND
                                AR.""ID_SUBFAMILIA"" = SFM.""ID_SUBFAMILIA"" AND
                                AR.""ID_COLOR"" = CL.""ID_COLOR"" AND
                                AR.""ID_ARTICULO_PRECIO"" = AP.""ID_ARTICULO_PRECIO"" AND
                                AR.""ID_ARTICULO_PRECIO"" = @id_articulo_precio
                            ORDER BY 
                                AR.""ID_ARTICULO"";
                        ";

    string query = select + fromAndWhere;

    List<Articulo> articulos = new List<Articulo>();

    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conex))
    {
        cmd.Parameters.AddWithValue("id_articulo_precio", articuloPrecioId);

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                articulos.Add(ReadArticulo(reader)); // Mapeo
            }
        }
    }

    return articulos;
}





            public List<Articulo> GetArticuloByFamiliaMedida(string subfamilia, string medida, NpgsqlConnection conex)
            {
                string selectText = $"SELECT AR.*, ";  
                selectText += "MD.\"CODIGO\" AS MEDIDA_CODIGO, MD.\"DESCRIPCION\" AS MEDIDA_DESCRIPCION, ";
                selectText += "SFM.\"CODIGO\" AS SUBFAMILIA_CODIGO, SFM.\"DESCRIPCION\" AS SUBFAMILIA_DESCRIPCION, ";
                selectText += "CL.\"CODIGO\" AS COLOR_CODIGO, CL.\"DESCRIPCION\" AS COLOR_DESCRIPCION, CL.\"HEXA\" AS COLOR_HEXA, ";
                selectText += "AP.\"CODIGO\" AS ARTICULO_PRECIO_CODIGO, AP.\"DESCRIPCION\" AS ARTICULO_PRECIO_DESCRIPCION ";

                string fromText = "FROM \"ARTICULO\" AR, \"MEDIDA\" MD, \"SUBFAMILIA\" SFM, \"COLOR\" CL, \"ARTICULO_PRECIO\" AP ";
                string whereText = "WHERE AR.\"ID_MEDIDA\" = MD.\"ID_MEDIDA\" AND AR.\"ID_SUBFAMILIA\" = SFM.\"ID_SUBFAMILIA\" AND ";
                whereText += "AR.\"ID_COLOR\" = CL.\"ID_COLOR\" AND AR.\"ID_ARTICULO_PRECIO\" = AP.\"ID_ARTICULO_PRECIO\" ";

                if (subfamilia != null)
                    whereText += "AND SFM.\"CODIGO\" = @id_subfamilia ";

                if (medida != null)
                    whereText += "AND MD.\"CODIGO\" = @id_medida ";

                string commandText = selectText + fromText + whereText;

                Console.WriteLine("Consulta: " + commandText + " MEDIDA= " + medida + " SUBFAMILIA= " + subfamilia);

                List<Articulo> articulos = new List<Articulo>();

                using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
                {
                    if (medida != null)
                        cmd.Parameters.AddWithValue("id_medida", medida);
                    if (subfamilia != null)
                        cmd.Parameters.AddWithValue("id_subfamilia", subfamilia);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            articulos.Add(ReadArticulo(reader)); // tu función para mapear un artículo
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
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO_PRECIO")),
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


public List<int> crearArticulos(Articulo[] articulos, Npgsql.NpgsqlConnection connection)
{
    var idsGenerados = new List<int>();

    string query = "INSERT INTO \"" + Articulo.TABLA + "\" (\"DESCRIPCION\", \"ID_ARTICULO_PRECIO\", \"CODIGO\", \"ID_COLOR\", \"ID_MEDIDA\", \"ID_SUBFAMILIA\", \"ID_FABRICANTE\") " +
                       "VALUES(@DESCRIPCION, @ID_ARTICULO_PRECIO, @CODIGO, @ID_COLOR, @ID_MEDIDA, @ID_SUBFAMILIA , @ID_FABRICANTE)";

    using (var cmd = new NpgsqlCommand())
    {
        cmd.Connection = connection;

        foreach (var articulo in articulos)
        {
            cmd.CommandText = query;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@DESCRIPCION", articulo.Descripcion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ID_ARTICULO_PRECIO", articulo.articuloPrecio.Id);
            cmd.Parameters.AddWithValue("@CODIGO", articulo.Codigo);
            cmd.Parameters.AddWithValue("@ID_COLOR", articulo.Color.Id);
            cmd.Parameters.AddWithValue("@ID_MEDIDA", articulo.Medida.Id);
            cmd.Parameters.AddWithValue("@ID_SUBFAMILIA", articulo.SubFamilia.Id);
            cmd.Parameters.AddWithValue("@ID_FABRICANTE", articulo.IdFabricante);

            var idGenerado = Convert.ToInt32(cmd.ExecuteScalar());
            idsGenerados.Add(idGenerado);
        }
    }

    return idsGenerados;
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
            string colorHexa = reader["COLOR_HEXA"] as string;   
   
            Color color =new Color
            {
                Id = colorId.Value,
                Codigo =    colorCodigo,
                Descripcion = colorDescripcion,
                ColorHexa = colorHexa
            };     


            int? subfamiliaId = reader["ID_" + SubFamilia.TABLA] as int?;            
            string subfamiliaCodigo = reader["SUBFAMILIA_CODIGO"] as string;   
            string subfamiliaDescripcion = reader["SUBFAMILIA_DESCRIPCION"] as string;   
            SubFamilia subfamilia = new SubFamilia
            {
                Id = subfamiliaId.Value,
                Codigo =    subfamiliaCodigo,
                Descripcion = subfamiliaDescripcion
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
                Id = articuloPrecioId.Value,
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
            articulo.SubFamilia = subfamilia;
            articulo.articuloPrecio = articuloPrecio;
            articulo.IdFabricante = idFabricante.Value;
            return articulo;
        }


    }

