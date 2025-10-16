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
    string query = "SELECT \"ID_ARTICULO_PRECIO\", \"CODIGO\", \"DESCRIPCION\", \"PRECIO1\", \"PRECIO2\", \"PRECIO3\", \"RELLENO\" FROM \"ARTICULO_PRECIO\"";
    
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
                    Precio3 = reader.IsDBNull(reader.GetOrdinal("PRECIO3")) ? 0m : reader.GetDecimal(reader.GetOrdinal("PRECIO3")),
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
public EstadisticaArticuloDTO GetArticuloPresupuestado(
    int idArticuloPrecio,
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    NpgsqlConnection connection)
{
    string query = @"
        SELECT ap.*, p.""FECHA_PRESUPUESTO""
        FROM ""ARTICULO_PRESUPUESTO"" ap
        INNER JOIN ""PRESUPUESTO"" p ON ap.""ID_PRESUPUESTO"" = p.""ID_PRESUPUESTO""
        WHERE ap.""ID_ARTICULO"" = @ID_ARTICULO
          AND (@FECHADESDE IS NULL OR p.""FECHA_PRESUPUESTO"" >= @FECHADESDE)
          AND (@FECHAHASTA IS NULL OR p.""FECHA_PRESUPUESTO"" <= @FECHAHASTA)";

    using var cmd = new NpgsqlCommand(query, connection);
    cmd.Parameters.Add("ID_ARTICULO", NpgsqlTypes.NpgsqlDbType.Integer).Value = idArticuloPrecio;
    cmd.Parameters.Add("FECHADESDE", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = (object?)fechaDesde ?? DBNull.Value;
    cmd.Parameters.Add("FECHAHASTA", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = (object?)fechaHasta ?? DBNull.Value;

    // Leer todas las filas en memoria
    int cantidadTotal = 0;
    DateTime fechaUltimoPresupuesto = DateTime.MinValue;

    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            cantidadTotal += reader.GetInt32(reader.GetOrdinal("CANTIDAD"));
            var fecha = reader.GetDateTime(reader.GetOrdinal("FECHA_PRESUPUESTO"));
            if (fecha > fechaUltimoPresupuesto)
                fechaUltimoPresupuesto = fecha; // opcional, si querés registrar la última fecha
        }
    }

    // Crear el DTO con un solo artículo
    var dto = new EstadisticaArticuloDTO
    {
        Articulo = GetArticulo(idArticuloPrecio, connection),
        CantidadPresupuestada = cantidadTotal,
    };

    return dto;
}


public List<int> ActualizarArticulosPrecios(ArticuloPrecio[] articuloPrecios, Npgsql.NpgsqlConnection connection)
{
    var filasAfectadas = new List<int>();

    if (connection.State != System.Data.ConnectionState.Open)
        connection.Open();

    foreach (var articulo in articuloPrecios)
    {
        int afectadas = 0;

        // 1️⃣ Actualizar precios
        using (var cmdPrecio = new NpgsqlCommand(
            $"UPDATE \"{ArticuloPrecio.TABLA}\" SET " +
            "\"PRECIO1\" = @PRECIO1, " +
            "\"PRECIO2\" = @PRECIO2, " +
            "\"PRECIO3\" = @PRECIO3 " +
            "WHERE \"CODIGO\" = @CODIGO", connection))
        {
            cmdPrecio.Parameters.AddWithValue("@PRECIO1", articulo.Precio1 ?? 0);
            cmdPrecio.Parameters.AddWithValue("@PRECIO2", articulo.Precio2 ?? 0);
            cmdPrecio.Parameters.AddWithValue("@PRECIO3", articulo.Precio3 ?? 0);
            cmdPrecio.Parameters.AddWithValue("@CODIGO", articulo.Codigo ?? (object)DBNull.Value);

            afectadas += cmdPrecio.ExecuteNonQuery();
        }

        // 2️⃣ Verificar si la descripción cambió
        string descripcionActual = null;

        using (var cmdSelect = new NpgsqlCommand(
            $"SELECT \"DESCRIPCION\" FROM \"{ArticuloPrecio.TABLA}\" WHERE \"CODIGO\" = @CODIGO", connection))
        {
            cmdSelect.Parameters.AddWithValue("@CODIGO", articulo.Codigo ?? (object)DBNull.Value);
            descripcionActual = cmdSelect.ExecuteScalar() as string;
        }

        bool descripcionDiferente = !string.Equals(
            articulo.Descripcion ?? "",
            descripcionActual ?? "",
            StringComparison.OrdinalIgnoreCase
        );

        if (descripcionDiferente)
        {
            // Actualizar en ARTICULO_PRECIO
            using (var cmdDescPrecio = new NpgsqlCommand(
                $"UPDATE \"{ArticuloPrecio.TABLA}\" SET \"DESCRIPCION\" = @DESCRIPCION WHERE \"CODIGO\" = @CODIGO", connection))
            {
                cmdDescPrecio.Parameters.AddWithValue("@DESCRIPCION", articulo.Descripcion ?? (object)DBNull.Value);
                cmdDescPrecio.Parameters.AddWithValue("@CODIGO", articulo.Codigo ?? (object)DBNull.Value);
                afectadas += cmdDescPrecio.ExecuteNonQuery();
            }

            // Actualizar en ARTICULO
            using (var cmdDescArticulo = new NpgsqlCommand(
                $"UPDATE \"ARTICULO\" SET \"DESCRIPCION\" = @DESCRIPCION WHERE \"CODIGO\" = @CODIGO", connection))
            {
                cmdDescArticulo.Parameters.AddWithValue("@DESCRIPCION", articulo.Descripcion ?? (object)DBNull.Value);
                cmdDescArticulo.Parameters.AddWithValue("@CODIGO", articulo.Codigo ?? (object)DBNull.Value);
                afectadas += cmdDescArticulo.ExecuteNonQuery();
            }
        }

        filasAfectadas.Add(afectadas);
    }

    return filasAfectadas;
}



public List<ConsultaMedida> ConsultarMedidasNecesarias(ArticuloPresupuesto[] presupuestosArticulos,NpgsqlConnection connection)
{
    var listaDeConsultasMedidas = new List<ConsultaMedida>();

    // 1. Obtener los ID_ARTICULO_PRECIO únicos
    var ids = presupuestosArticulos
        .Select(pa => pa.Articulo.articuloPrecio.Id)
        .Where(id => id != null)
        .Distinct()
        .ToArray();

    // 2. Traer todos los precios en un solo query
    var preciosPorId = ObtenerPreciosPorIds(ids, connection);

    // 3. Recorrer los artículos del presupuesto
    foreach (var presuArt in presupuestosArticulos)
    {
        if (presuArt?.Articulo?.Medida == null)
            continue;

        string codigoMedida = presuArt.Articulo.Medida.Codigo;
        int cantidad = presuArt.cantidad;

        // Buscar el relleno desde el diccionario
        int relleno = 1;
        if (preciosPorId.TryGetValue(presuArt.Articulo.articuloPrecio.Id, out var articuloPrecio))
        {
            relleno = (int?)articuloPrecio.Relleno ?? 1;
        }

        int cantidadFinal = cantidad * relleno;

        // Acumular en lista
        var existente = listaDeConsultasMedidas.FirstOrDefault(cm => cm.Medida == codigoMedida);
        if (existente != null)
        {
            existente.Cantidad += cantidadFinal;
        }
        else
        {
            listaDeConsultasMedidas.Add(new ConsultaMedida
            {
                Medida = codigoMedida,
                Cantidad = cantidadFinal
            });
        }
    }

    return listaDeConsultasMedidas;
}



public Dictionary<int, ArticuloPrecio> ObtenerPreciosPorIds(int[] ids, NpgsqlConnection connection)
{
    var resultado = new Dictionary<int, ArticuloPrecio>();

    if (ids == null || ids.Length == 0)
        return resultado;

    string query = @"
        SELECT 
            ""ID_ARTICULO_PRECIO"",
            ""CODIGO"",
            ""DESCRIPCION"",
            ""PRECIO1"",
            ""PRECIO2"",
            ""PRECIO3"",
            ""RELLENO""
        FROM ""ARTICULO_PRECIO""
        WHERE ""ID_ARTICULO_PRECIO"" = ANY(@ids)";

    using (var cmd = new NpgsqlCommand(query, connection))
    {
        cmd.Parameters.AddWithValue("@ids", ids);

        if (connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var precio = new ArticuloPrecio
                {
                    Id = reader.GetInt32(0),
                    Codigo = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio1 = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                    Precio2 = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    Precio3 = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                    Relleno = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                };

                resultado[precio.Id] = precio;
            }
        }
    }

    return resultado;
}

public List<ConsultaTallerCortePorCodigo> ConsultarTodosArticulosCantidadesTallerCorte(NpgsqlConnection connection)
{
    return ObtenerTallerCorteSeparadoAgrupadoPorCodigo("", null, connection);
}

public List<ConsultaTallerCortePorCodigo> ConsultarCantidadesTallerCorte(string codigo, NpgsqlConnection connection)
{
    return ObtenerTallerCorteSeparadoAgrupadoPorCodigo(
        " a.\"CODIGO\" = @codigo",
        cmd => cmd.Parameters.AddWithValue("@codigo", codigo),
        connection
    );
}



private List<ConsultaTallerCorte> EjecutarConsultaTallerCorteSeparado(
    string whereClause,
    Action<NpgsqlCommand>? addParams,
    NpgsqlConnection connection)
{
    var resultado = new List<ConsultaTallerCorte>();

    // 1️⃣ Query principal: Corte y Taller
    string queryCorteTaller = $@"
        SELECT
            a.""ID_ARTICULO"",
            a.""CODIGO"",
            a.""DESCRIPCION"" || ' ' || COALESCE(c.""DESCRIPCION"", '') AS ""DescripcionCompleta"",
            a.""STOCK"",
            c.""CODIGO"" AS ""CodigoColor"",
            c.""DESCRIPCION"" AS ""DescripcionColor"",
            c.""HEXA"" AS ""HexaColor"",
            COALESCE(SUM(CASE WHEN pp.""ID_ESTADO_PEDIDO_PROD"" = 2 THEN pa.""CANTIDAD"" ELSE 0 END), 0) AS ""CantidadEnCorte"",
            COALESCE(SUM(CASE WHEN pp.""ID_ESTADO_PEDIDO_PROD"" = 3 THEN pa.""CANTIDAD"" ELSE 0 END), 0) AS ""CantidadEnTaller""
        FROM ""PRODUCCION_ARTICULO"" pa
        LEFT JOIN ""ARTICULO"" a ON a.""ID_ARTICULO"" = pa.""ID_ARTICULO""
        LEFT JOIN ""COLOR"" c ON a.""ID_COLOR"" = c.""ID_COLOR""
        LEFT JOIN ""PEDIDO_PRODUCCION"" pp ON pa.""ID_PEDIDO_PRODUCCION"" = pp.""ID_PEDIDO_PRODUCCION""
        {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
        GROUP BY 
            a.""ID_ARTICULO"",
            a.""CODIGO"",
            a.""DESCRIPCION"",
            a.""STOCK"",
            c.""CODIGO"",
            c.""DESCRIPCION"",
            c.""HEXA""
        ORDER BY a.""ID_ARTICULO"";";

    using (var cmd = new NpgsqlCommand(queryCorteTaller, connection))
    {
        addParams?.Invoke(cmd);

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var articulo = new Articulo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO")),
                    Codigo = reader.GetString(reader.GetOrdinal("CODIGO")),
                    Descripcion = reader.GetString(reader.GetOrdinal("DescripcionCompleta")),
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

    // 2️⃣ Query separada: Cantidad separada (ARTICULO_PRESUPUESTO)
    string querySeparadas = $@"
        SELECT 
            a.""ID_ARTICULO"",
            COALESCE(SUM(CASE WHEN ap.""HAY_STOCK"" = TRUE THEN ap.""CANTIDAD"" ELSE 0 END), 0) AS ""CantidadSeparada""
        FROM ""ARTICULO"" a
        LEFT JOIN ""ARTICULO_PRESUPUESTO"" ap ON a.""ID_ARTICULO"" = ap.""ID_ARTICULO""
        LEFT JOIN ""PRESUPUESTO"" p ON ap.""ID_PRESUPUESTO"" = p.""ID_PRESUPUESTO"" AND p.""ID_ESTADO"" = 2
        {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
        GROUP BY a.""ID_ARTICULO""
        ORDER BY a.""ID_ARTICULO"";";

    var cantidadesSeparadas = new Dictionary<int, int>();
    using (var cmdSep = new NpgsqlCommand(querySeparadas, connection))
    {
        // ✅ Pasamos los mismos parámetros que la primera query
        addParams?.Invoke(cmdSep);

        using var reader = cmdSep.ExecuteReader();
        while (reader.Read())
        {
            int idArticulo = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO"));
            int cantidadSeparada = reader.GetInt32(reader.GetOrdinal("CantidadSeparada"));
            cantidadesSeparadas[idArticulo] = cantidadSeparada;
        }
    }

    // 3️⃣ Combinar resultados
    foreach (var item in resultado)
    {
        if (cantidadesSeparadas.TryGetValue(item.articulo.Id, out int cantidadSeparada))
        {
            item.CantidadSeparadoUnitario = cantidadSeparada;
        }
        else
        {
            item.CantidadSeparadoUnitario = 0;
        }
    }

    return resultado;
}



public List<ConsultaTallerCortePorCodigo> ObtenerTallerCorteSeparadoAgrupadoPorCodigo(string whereClause, Action<NpgsqlCommand>? addParams, NpgsqlConnection connection)
{
    var consultasIndividuales = EjecutarConsultaTallerCorteSeparado(whereClause, addParams, connection);

    var agrupado = consultasIndividuales
        .GroupBy(c => c.articulo.Codigo)
        .Select(grupo => new ConsultaTallerCortePorCodigo
        {
            Codigo = grupo.Key,
            CantidadEnCorteTotal = grupo.Sum(x => x.CantidadEnCorteUnitario),
            CantidadEnTallerTotal = grupo.Sum(x => x.CantidadEnTallerUnitario),
            CantidadSeparadoTotal = grupo.Sum(x => x.CantidadSeparadoUnitario),
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


public int ActualizarStock(ActualizacionStockInutDTO[] articulos, NpgsqlConnection connection)
    {
        int cantidadTotal = 0;

        using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var articulo in articulos)
            {
                using var cmd = new NpgsqlCommand(
                    @"UPDATE ""ARTICULO"" 
                      SET ""STOCK"" = @stock 
                      WHERE ""ID_ARTICULO"" = @idArticulo", connection, transaction);

                cmd.Parameters.AddWithValue("@stock", articulo.CantidadStock);
                cmd.Parameters.AddWithValue("@idArticulo", articulo.IdArticulo);

                cantidadTotal += cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        return cantidadTotal;
    }

    }

