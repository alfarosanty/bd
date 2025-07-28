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

        public List<Articulo> getAll(NpgsqlConnection conex)
        {
            string selectText = NewMethod(); // devuelve SELECT AR.*, MD."CODIGO" AS MEDIDA_CODIGO, ...
            
            string fromAndWhere = @"
                FROM 
                    ""ARTICULO"" AR
                    JOIN ""MEDIDA"" MD ON AR.""ID_MEDIDA"" = MD.""ID_MEDIDA""
                    JOIN ""SUBFAMILIA"" SFM ON AR.""ID_SUBFAMILIA"" = SFM.""ID_SUBFAMILIA""
                    JOIN ""COLOR"" CL ON AR.""ID_COLOR"" = CL.""ID_COLOR""
                    JOIN ""ARTICULO_PRECIO"" AP ON AR.""ID_ARTICULO_PRECIO"" = AP.""ID_ARTICULO_PRECIO""
                ORDER BY 
                    AR.""ID_ARTICULO""";

            string commandText = selectText + " " + fromAndWhere;

            List<Articulo> articulos = new List<Articulo>();

            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
            {
                Console.WriteLine("Consulta: " + commandText);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        articulos.Add(ReadArticulo(reader));
                    }
                }
            }

            return articulos;
        }





public List<Articulo> GetArticulosByArticuloPrecioId(int articuloPrecioId, bool habilitados, NpgsqlConnection conex)
{
    string select = NewMethod(); // SELECT reutilizable

    string habilitadoFiltro = habilitados ? @"AND AR.""HABILITADO"" = TRUE" : "";

    string fromAndWhere = $@"
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
            {habilitadoFiltro}
        ORDER BY 
            AR.""ID_ARTICULO"";
    ";

    string query = select + fromAndWhere;

    var articulos = new List<Articulo>();

    using (var cmd = new NpgsqlCommand(query, conex))
    {
        cmd.Parameters.AddWithValue("id_articulo_precio", articuloPrecioId);

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                articulos.Add(ReadArticulo(reader));
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

    string insertQuery = "INSERT INTO \"" + Articulo.TABLA + "\" " +
                         "(\"DESCRIPCION\", \"ID_ARTICULO_PRECIO\", \"CODIGO\", \"ID_COLOR\", \"ID_MEDIDA\", \"ID_SUBFAMILIA\", \"ID_FABRICANTE\", \"HABILITADO\" , \"STOCK\") " +
                         "VALUES (@DESCRIPCION, @ID_ARTICULO_PRECIO, @CODIGO, @ID_COLOR, @ID_MEDIDA, @ID_SUBFAMILIA, @ID_FABRICANTE, @HABILITADO, @STOCK) " +
                         "RETURNING \"ID_ARTICULO\"";

    string updateQuery = "UPDATE \"" + Articulo.TABLA + "\" " +
                         "SET \"HABILITADO\" = @HABILITADO " +
                         "WHERE \"ID_ARTICULO\" = @ID_ARTICULO";

    using (var cmd = new NpgsqlCommand())
    {
        cmd.Connection = connection;

        foreach (var articulo in articulos)
        {
            cmd.Parameters.Clear();

            if (articulo.Nuevo == true)
            {
                cmd.CommandText = insertQuery;

                cmd.Parameters.AddWithValue("@DESCRIPCION", articulo.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_ARTICULO_PRECIO", articulo.articuloPrecio.Id);
                cmd.Parameters.AddWithValue("@CODIGO", articulo.Codigo);
                cmd.Parameters.AddWithValue("@ID_COLOR", articulo.Color.Id);
                cmd.Parameters.AddWithValue("@ID_MEDIDA", articulo.Medida.Id);
                cmd.Parameters.AddWithValue("@ID_SUBFAMILIA", articulo.SubFamilia.Id);
                cmd.Parameters.AddWithValue("@ID_FABRICANTE", articulo.IdFabricante);
                cmd.Parameters.AddWithValue("@HABILITADO", articulo.Habilitado);
                cmd.Parameters.AddWithValue("@STOCK", 0);



                var idGenerado = Convert.ToInt32(cmd.ExecuteScalar());
                idsGenerados.Add(idGenerado);
            }
            else
            {
                cmd.CommandText = updateQuery;

                cmd.Parameters.AddWithValue("@HABILITADO", articulo.Habilitado ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_ARTICULO", articulo.Id); // debe tener el ID asignado

                cmd.ExecuteNonQuery();
                idsGenerados.Add(articulo.Id);
            }
        }
    }

    return idsGenerados;
}

public List<int> CrearArticulosPrecios(ArticuloPrecio[] articuloPrecios, Npgsql.NpgsqlConnection connection)
{
    var idsGenerados = new List<int>();

    string insertQuery = "INSERT INTO \"" + ArticuloPrecio.TABLA + "\" " +
                         "(\"CODIGO\", \"DESCRIPCION\", \"PRECIO1\", \"PRECIO2\", \"PRECIO3\") " +
                         "VALUES (@CODIGO, @DESCRIPCION, @PRECIO1, @PRECIO2, @PRECIO3) " +
                         "RETURNING \"ID_ARTICULO_PRECIO\"";

    using (var cmd = new NpgsqlCommand(insertQuery, connection))
    {
        // Asegurarse de que la conexión esté abierta
        if (connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        foreach (var articuloPrecio in articuloPrecios)
        {
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@CODIGO", articuloPrecio.Codigo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DESCRIPCION", articuloPrecio.Descripcion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PRECIO1", articuloPrecio.Precio1 ?? 0);
            cmd.Parameters.AddWithValue("@PRECIO2", articuloPrecio.Precio2 ?? 0);
            cmd.Parameters.AddWithValue("@PRECIO3", articuloPrecio.Precio3 ?? 0);


            try
            {
                var idGenerado = Convert.ToInt32(cmd.ExecuteScalar());
                idsGenerados.Add(idGenerado);
            }
            catch (Exception ex)
            {
                // Manejo de errores opcional
                Console.WriteLine($"Error al insertar artículo: {ex.Message}");
                // Podés optar por continuar o lanzar la excepción
                // throw;
            }
        }
    }

    return idsGenerados;
}

public List<int> ActualizarArticulosPrecios(ArticuloPrecio[] articuloPrecios, Npgsql.NpgsqlConnection connection)
{
    var filasAfectadasPorArticulo = new List<int>();

    var descripcionesActuales = new Dictionary<string, string>();
    string selectQuery = $"SELECT \"CODIGO\", \"DESCRIPCION\" FROM \"{ArticuloPrecio.TABLA}\" WHERE \"CODIGO\" = ANY(@codigos)";
    
    using (var cmdSelect = new NpgsqlCommand(selectQuery, connection))
    {
        if (connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        var codigos = articuloPrecios.Select(ap => ap.Codigo).ToArray();
        cmdSelect.Parameters.AddWithValue("@codigos", codigos);

        using (var reader = cmdSelect.ExecuteReader())
        {
            while (reader.Read())
            {
                string codigo = reader.GetString(0);
                string descripcion = reader.IsDBNull(1) ? null : reader.GetString(1);
                descripcionesActuales[codigo] = descripcion;
            }
        }
    }

    string updatePrecioQuery = $"UPDATE \"{ArticuloPrecio.TABLA}\" SET " +
                              "\"PRECIO1\" = @PRECIO1, " +
                              "\"PRECIO2\" = @PRECIO2, " +
                              "\"PRECIO3\" = @PRECIO3 " +
                              "WHERE \"CODIGO\" = @CODIGO";

    string updateDescripcionPrecioQuery = $"UPDATE \"{ArticuloPrecio.TABLA}\" SET \"DESCRIPCION\" = @DESCRIPCION " +
                                          "WHERE \"CODIGO\" = @CODIGO";

    string updateDescripcionArticuloQuery = "UPDATE \"ARTICULO\" SET \"DESCRIPCION\" = @DESCRIPCION WHERE \"CODIGO\" = @CODIGO";

    using (var cmdPrecio = new NpgsqlCommand(updatePrecioQuery, connection))
    using (var cmdDescripcionPrecio = new NpgsqlCommand(updateDescripcionPrecioQuery, connection))
    using (var cmdDescripcionArticulo = new NpgsqlCommand(updateDescripcionArticuloQuery, connection))
    {
        foreach (var articuloPrecio in articuloPrecios)
        {
            bool actualizoDescripcion = false;

            string descripcionActual = null;
            descripcionesActuales.TryGetValue(articuloPrecio.Codigo, out descripcionActual);

            // Comparación SIN trim
            bool descripcionDiferente = !string.Equals(
                articuloPrecio.Descripcion ?? "",
                descripcionActual ?? "",
                StringComparison.OrdinalIgnoreCase
            );

            try
            {
                cmdPrecio.Parameters.Clear();
                cmdPrecio.Parameters.AddWithValue("@PRECIO1", articuloPrecio.Precio1 ?? 0);
                cmdPrecio.Parameters.AddWithValue("@PRECIO2", articuloPrecio.Precio2 ?? 0);
                cmdPrecio.Parameters.AddWithValue("@PRECIO3", articuloPrecio.Precio3 ?? 0);
                cmdPrecio.Parameters.AddWithValue("@CODIGO", articuloPrecio.Codigo ?? (object)DBNull.Value);
                int filasAfectadas = cmdPrecio.ExecuteNonQuery();

                if (descripcionDiferente)
                {
                    cmdDescripcionPrecio.Parameters.Clear();
                    cmdDescripcionPrecio.Parameters.AddWithValue("@DESCRIPCION", articuloPrecio.Descripcion ?? (object)DBNull.Value);
                    cmdDescripcionPrecio.Parameters.AddWithValue("@CODIGO", articuloPrecio.Codigo ?? (object)DBNull.Value);
                    filasAfectadas += cmdDescripcionPrecio.ExecuteNonQuery();

                    cmdDescripcionArticulo.Parameters.Clear();
                    cmdDescripcionArticulo.Parameters.AddWithValue("@DESCRIPCION", articuloPrecio.Descripcion ?? (object)DBNull.Value);
                    cmdDescripcionArticulo.Parameters.AddWithValue("@CODIGO", articuloPrecio.Codigo ?? (object)DBNull.Value);
                    filasAfectadas += cmdDescripcionArticulo.ExecuteNonQuery();
                }

                filasAfectadasPorArticulo.Add(filasAfectadas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar artículo con código {articuloPrecio.Codigo}: {ex.Message}");
                filasAfectadasPorArticulo.Add(0);
            }
        }
    }

    return filasAfectadasPorArticulo;
}



public List<ConsultaMedida> ConsultarMedidasNecesarias(ArticuloPresupuesto[] presupuestosArticulos)
{
    var listaDeConsultasMedidas = new List<ConsultaMedida>();

    foreach (var presuArt in presupuestosArticulos)
    {
        var consultaMedidaExistente = listaDeConsultasMedidas
            .FirstOrDefault(cm => cm.Medida == presuArt.Articulo.Medida.Codigo);

        if (consultaMedidaExistente != null)
        {
            consultaMedidaExistente.Cantidad += presuArt.cantidad;
        }
        else
        {
            listaDeConsultasMedidas.Add(new ConsultaMedida
            {
                Medida = presuArt.Articulo.Medida.Codigo,
                Cantidad = presuArt.cantidad
            });
        }
    }

    return listaDeConsultasMedidas;
}

public List<ConsultaTallerCortePorCodigo> ConsultarTodosArticulosCantidadesTallerCorte(NpgsqlConnection connection)
{
    return ObtenerTallerCorteAgrupadoPorCodigo("", null, connection);
}

public List<ConsultaTallerCortePorCodigo> ConsultarCantidadesTallerCorte(int idArticuloPrecio, NpgsqlConnection connection)
{
    return ObtenerTallerCorteAgrupadoPorCodigo(
        "WHERE ar.\"ID_ARTICULO_PRECIO\" = @idArticuloPrecio",
        cmd => cmd.Parameters.AddWithValue("@idArticuloPrecio", idArticuloPrecio),
        connection
    );
}



private List<ConsultaTallerCorte> EjecutarConsultaTallerCorte(string whereClause, Action<NpgsqlCommand>? addParams, NpgsqlConnection connection)
{
    var resultado = new List<ConsultaTallerCorte>();

    string query = $@"
SELECT 
    ar.""ID_ARTICULO"",
    ar.""CODIGO"",
    ar.""DESCRIPCION"",
    ar.""ID_COLOR"",
    ar.""STOCK"",
    c.""CODIGO"" AS ""CodigoColor"",
    c.""DESCRIPCION"" AS ""DescripcionColor"",
    c.""HEXA"" AS ""HexaColor"",
    COALESCE(SUM(CASE WHEN pp.""ID_ESTADO_PEDIDO_PROD"" = 2 THEN ppa.""CANTIDAD"" ELSE 0 END), 0) AS ""CantidadEnCorte"",
    COALESCE(SUM(CASE WHEN pp.""ID_ESTADO_PEDIDO_PROD"" = 3 THEN ppa.""CANTIDAD"" ELSE 0 END), 0) AS ""CantidadEnTaller""
FROM ""{Articulo.TABLA}"" ar
LEFT JOIN ""{Color.TABLA}"" c ON c.""ID_COLOR"" = ar.""ID_COLOR""
LEFT JOIN ""{PedidoProduccionArticulo.TABLA}"" ppa ON ppa.""ID_ARTICULO"" = ar.""ID_ARTICULO""
LEFT JOIN ""{PedidoProduccion.TABLA}"" pp ON pp.""ID_PEDIDO_PRODUCCION"" = ppa.""ID_PEDIDO_PRODUCCION""
{whereClause}
GROUP BY ar.""ID_ARTICULO"", ar.""CODIGO"", ar.""DESCRIPCION"", ar.""ID_COLOR"", ar.""STOCK"", c.""CODIGO"", c.""DESCRIPCION"", c.""HEXA""
ORDER BY ar.""ID_ARTICULO"";
";



    using (var cmd = new NpgsqlCommand(query, connection))
    {
        addParams?.Invoke(cmd); // agrega parámetros si hace falta

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var articulo = new Articulo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO")),
                    Codigo = reader.GetString(reader.GetOrdinal("CODIGO")),
                    Descripcion = reader.GetString(reader.GetOrdinal("DESCRIPCION")),
                    // Aquí asumimos que tienes un objeto Color dentro de Articulo:
                    Color = new Color
                    {
                        Codigo = reader.GetString(reader.GetOrdinal("CodigoColor")),
                        Descripcion = reader.GetString(reader.GetOrdinal("DescripcionColor")),
                        ColorHexa = reader.IsDBNull(reader.GetOrdinal("HexaColor")) 
                            ? "" 
                            : reader.GetString(reader.GetOrdinal("HexaColor"))
                    }

                };


                int cantidadEnCorte = Convert.ToInt32(reader["CantidadEnCorte"]);
                int cantidadEnTaller = Convert.ToInt32(reader["CantidadEnTaller"]);
                int stockUnitario = reader.GetInt32(reader.GetOrdinal("STOCK"));


                resultado.Add(new ConsultaTallerCorte
                {
                    articulo = articulo,
                    CantidadEnCorteUnitario = cantidadEnCorte,
                    CantidadEnTallerUnitario = cantidadEnTaller,
                    StockUnitario = stockUnitario
                });
            }
        }
    }

    return resultado;
}

public List<ConsultaTallerCortePorCodigo> ObtenerTallerCorteAgrupadoPorCodigo(string whereClause, Action<NpgsqlCommand>? addParams, NpgsqlConnection connection)
{
    var consultasIndividuales = EjecutarConsultaTallerCorte(whereClause, addParams, connection);

    var agrupado = consultasIndividuales
        .GroupBy(c => c.articulo.Codigo)
        .Select(grupo => new ConsultaTallerCortePorCodigo
        {
            Codigo = grupo.Key,
            CantidadEnCorteTotal = grupo.Sum(x => x.CantidadEnCorteUnitario),
            CantidadEnTallerTotal = grupo.Sum(x => x.CantidadEnTallerUnitario),
            StockTotal = grupo.Sum(x => x.StockUnitario),
            Consultas = grupo.ToList()
        })
        .ToList();

    return agrupado;
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

            bool? habilitado = reader["HABILITADO"] != DBNull.Value ? (bool?)reader["HABILITADO"] : null;

            int? stock = reader["STOCK"] != DBNull.Value ? Convert.ToInt32(reader["STOCK"]) : (int?)null;


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
            articulo.Habilitado = habilitado;
            articulo.Nuevo = false;
            articulo.Stock = stock;
            return articulo;
        }


    }

