using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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
    string query = "SELECT \"ID_ARTICULO_PRECIO\", \"CODIGO\", \"DESCRIPCION\", \"PRECIO1\", \"PRECIO2\", \"PRECIO3\", \"RELLENO\" FROM \"ARTICULO_PRECIO\" ORDER BY \"ID_ARTICULO_PRECIO\"" ;
    
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
                    Relleno = reader.IsDBNull(reader.GetOrdinal("RELLENO")) ? 0 : reader.GetInt32(reader.GetOrdinal("RELLENO")),

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

public List<int> ActualizarArticulosPrecios(
    ArticuloPrecio[] articuloPrecios,
    int usuarioId,
    NpgsqlConnection connection)
{
    var filasAfectadas = new List<int>();

    if (connection.State != ConnectionState.Open)
        connection.Open();

    using var tx = connection.BeginTransaction();

    // 🔑 Pasamos el usuario a PostgreSQL (lo usa el trigger)
    using (var cmdUser = new NpgsqlCommand(
        "SET LOCAL app.usuario_id = @usuarioId", connection, tx))
    {
        cmdUser.Parameters.AddWithValue("@usuarioId", usuarioId);
        cmdUser.ExecuteNonQuery();
    }

    foreach (var articulo in articuloPrecios)
    {
        int afectadas = 0;

        // 1️⃣ Actualizar precios
        using (var cmdPrecio = new NpgsqlCommand(
            $"UPDATE \"{ArticuloPrecio.TABLA}\" SET " +
            "\"PRECIO1\" = @PRECIO1, " +
            "\"PRECIO2\" = @PRECIO2, " +
            "\"PRECIO3\" = @PRECIO3 " +
            "WHERE \"CODIGO\" = @CODIGO", connection, tx))
        {
            cmdPrecio.Parameters.AddWithValue("@PRECIO1", articulo.Precio1 ?? 0);
            cmdPrecio.Parameters.AddWithValue("@PRECIO2", articulo.Precio2 ?? 0);
            cmdPrecio.Parameters.AddWithValue("@PRECIO3", articulo.Precio3 ?? 0);
            cmdPrecio.Parameters.AddWithValue("@CODIGO", articulo.Codigo ?? (object)DBNull.Value);

            afectadas += cmdPrecio.ExecuteNonQuery();
        }

        // 2️⃣ Verificar descripción
        string descripcionActual;

        using (var cmdSelect = new NpgsqlCommand(
            $"SELECT \"DESCRIPCION\" FROM \"{ArticuloPrecio.TABLA}\" WHERE \"CODIGO\" = @CODIGO",
            connection, tx))
        {
            cmdSelect.Parameters.AddWithValue("@CODIGO", articulo.Codigo ?? (object)DBNull.Value);
            descripcionActual = cmdSelect.ExecuteScalar() as string;
        }

        if (!string.Equals(
            articulo.Descripcion ?? "",
            descripcionActual ?? "",
            StringComparison.OrdinalIgnoreCase))
        {
            using (var cmdDescPrecio = new NpgsqlCommand(
                $"UPDATE \"{ArticuloPrecio.TABLA}\" SET \"DESCRIPCION\" = @DESCRIPCION WHERE \"CODIGO\" = @CODIGO",
                connection, tx))
            {
                cmdDescPrecio.Parameters.AddWithValue("@DESCRIPCION", articulo.Descripcion ?? (object)DBNull.Value);
                cmdDescPrecio.Parameters.AddWithValue("@CODIGO", articulo.Codigo ?? (object)DBNull.Value);
                afectadas += cmdDescPrecio.ExecuteNonQuery();
            }

            using (var cmdDescArticulo = new NpgsqlCommand(
                $"UPDATE \"ARTICULO\" SET \"DESCRIPCION\" = @DESCRIPCION WHERE \"CODIGO\" = @CODIGO",
                connection, tx))
            {
                cmdDescArticulo.Parameters.AddWithValue("@DESCRIPCION", articulo.Descripcion ?? (object)DBNull.Value);
                cmdDescArticulo.Parameters.AddWithValue("@CODIGO", articulo.Codigo ?? (object)DBNull.Value);
                afectadas += cmdDescArticulo.ExecuteNonQuery();
            }
        }

        filasAfectadas.Add(afectadas);
    }

    tx.Commit();
    return filasAfectadas;
}




public List<ConsultaMedida> ConsultarMedidasNecesarias(ArticuloPresupuesto[] presupuestosArticulos,NpgsqlConnection connection)
{
    var listaDeConsultasMedidas = new List<ConsultaMedida>();


    foreach (var presuArt in presupuestosArticulos)
    {
        if (presuArt?.Articulo?.Medida == null)
            continue;

        string codigoMedida = presuArt.Articulo.Medida.Codigo;
        int cantidad = presuArt.cantidad;

        // Buscar el relleno desde el diccionario
        int relleno = presuArt.Articulo.articuloPrecio.Relleno ?? 1;

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
    return ConsultarCantidadesTallerCorte("", connection);
}

public List<ConsultaTallerCortePorCodigo> ConsultarCantidadesTallerCorte(string codigo, NpgsqlConnection connection)
{
    var stock = consultarStock(codigo, connection);
    var enCorte = consultarEnCorte(codigo, connection);
    var enTaller = consultarEnTaller(codigo, connection);
    var separados = consultarSeparados(codigo, connection);

    var resultado = new List<ConsultaTallerCorte>();

    foreach (var s in stock)
    {
        var articulo = s.articulo;

        var corte = enCorte.FirstOrDefault(x => x.articulo.Id == articulo.Id);
        var taller = enTaller.FirstOrDefault(x => x.articulo.Id == articulo.Id);
        var separado = separados.FirstOrDefault(x => x.articulo.Id == articulo.Id);

        var consulta = new ConsultaTallerCorte
        {
            articulo = articulo,
            StockUnitario = s.StockUnitario,
            CantidadEnCorteUnitario = corte?.CantidadEnCorteUnitario ?? 0,
            CantidadEnTallerUnitario = taller?.CantidadEnTallerUnitario ?? 0,
            CantidadSeparadoUnitario = separado?.CantidadSeparadoUnitario ?? 0,
            CantidadEstanteriaUnitario = s.StockUnitario - (separado?.CantidadSeparadoUnitario ?? 0)
        };

        resultado.Add(consulta);
    }

    // Si se pidió un código específico, devolvemos una sola agrupación
    if (!string.IsNullOrEmpty(codigo))
    {
        var agrupado = new ConsultaTallerCortePorCodigo
        {
            Codigo = codigo,
            StockTotal = resultado.Sum(x => x.StockUnitario),
            CantidadEnCorteTotal = resultado.Sum(x => x.CantidadEnCorteUnitario),
            CantidadEnTallerTotal = resultado.Sum(x => x.CantidadEnTallerUnitario),
            CantidadSeparadoTotal = resultado.Sum(x => x.CantidadSeparadoUnitario),
            CantidadEstanteriaTotal = resultado.Sum(x => x.CantidadEstanteriaUnitario),
            Consultas = resultado
        };

        return new List<ConsultaTallerCortePorCodigo> { agrupado };
    }

    // Si no se pidió un código, agrupamos por cada código real de artículo
    var agrupadoPorCodigo = resultado
        .GroupBy(r => r.articulo.Codigo)
        .Select(g => new ConsultaTallerCortePorCodigo
        {
            Codigo = g.Key,
            StockTotal = g.Sum(x => x.StockUnitario),
            CantidadEnCorteTotal = g.Sum(x => x.CantidadEnCorteUnitario),
            CantidadEnTallerTotal = g.Sum(x => x.CantidadEnTallerUnitario),
            CantidadSeparadoTotal = g.Sum(x => x.CantidadSeparadoUnitario),
            CantidadEstanteriaTotal = g.Sum(x => x.CantidadEstanteriaUnitario),
            Consultas = g.ToList()
        })
        .ToList();

    return agrupadoPorCodigo;
}


private List<ConsultaTallerCorte> consultarStock(string? codigo, NpgsqlConnection connection)
{
    var resultado = new List<ConsultaTallerCorte>();

    string query = @"
        SELECT 
            a.""ID_ARTICULO"", 
            a.""CODIGO"" AS ""CodigoArticulo"",
            a.""DESCRIPCION"" AS ""DescripcionArticulo"",  
            c.""CODIGO"" AS ""CodigoColor"", 
            c.""DESCRIPCION"" AS ""DescripcionColor"",
            c.""HEXA"" AS ""ColorHexa"",  
            c.""ID_COLOR"",
            a.""STOCK""
        FROM PUBLIC.""ARTICULO"" a
        JOIN PUBLIC.""COLOR"" c ON a.""ID_COLOR"" = c.""ID_COLOR""
        /**where**/
        GROUP BY a.""ID_ARTICULO"", a.""CODIGO"", a.""DESCRIPCION"", c.""CODIGO"", a.""STOCK"", c.""ID_COLOR"", c.""HEXA"", c.""DESCRIPCION""
        ORDER BY a.""ID_ARTICULO"";";

    // Agregar condición opcional
    if (!string.IsNullOrEmpty(codigo))
        query = query.Replace("/**where**/", "WHERE a.\"CODIGO\" = @codigo");
    else
        query = query.Replace("/**where**/", "");

    using (var command = new NpgsqlCommand(query, connection))
    {
        if (!string.IsNullOrEmpty(codigo))
            command.Parameters.AddWithValue("@codigo", codigo);

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var color = new Color
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_COLOR")),
                    Codigo = reader.GetString(reader.GetOrdinal("CodigoColor")),
                    Descripcion = reader.GetString(reader.GetOrdinal("DescripcionColor")),
                    ColorHexa = reader["ColorHexa"] == DBNull.Value ? null : reader["ColorHexa"].ToString(),

                };
                var articulo = new Articulo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO")),
                    Codigo = reader.GetString(reader.GetOrdinal("CodigoArticulo")),
                    Descripcion = reader.GetString(reader.GetOrdinal("DescripcionArticulo")),
                    Color = color
                };

                var consulta = new ConsultaTallerCorte
                {
                    articulo = articulo,
                    StockUnitario = reader.GetInt32(reader.GetOrdinal("STOCK")),
                    CantidadEnCorteUnitario = 0,
                    CantidadEnTallerUnitario = 0,
                    CantidadSeparadoUnitario = 0,
                    CantidadEstanteriaUnitario = 0
                };

                resultado.Add(consulta);
            }
        }
    }

    return resultado;
}

private List<ConsultaTallerCorte> consultarEnCorte(string? codigo, NpgsqlConnection connection)
{
    var resultado = new List<ConsultaTallerCorte>();

    string query = @"
        SELECT 
            a.""ID_ARTICULO"",
            pa.""CODIGO"" AS ""CodigoArticulo"",
            c.""ID_COLOR"",
            c.""CODIGO"" AS ""CodigoColor"",
            SUM(pa.""CANTIDAD"") AS ""CORTE""
        FROM PUBLIC.""PEDIDO_PRODUCCION"" pp
        JOIN PUBLIC.""PRODUCCION_ARTICULO"" pa ON pp.""ID_PEDIDO_PRODUCCION"" = pa.""ID_PEDIDO_PRODUCCION""
        JOIN PUBLIC.""ARTICULO"" a ON a.""ID_ARTICULO"" = pa.""ID_ARTICULO""
        JOIN PUBLIC.""COLOR"" c ON a.""ID_COLOR"" = c.""ID_COLOR""
        JOIN PUBLIC.""FABRICANTE"" f ON f.""ID_FABRICANTE"" = pp.""ID_FABRICANTE""
        WHERE pp.""ID_ESTADO_PEDIDO_PROD"" = 2
        AND f.""APORTA_STOCK"" = TRUE

        /**where**/
        GROUP BY a.""ID_ARTICULO"", pa.""CODIGO"", c.""ID_COLOR"", c.""CODIGO"", c.""ID_COLOR""
        ORDER BY a.""ID_ARTICULO"";";

    if (!string.IsNullOrEmpty(codigo))
        query = query.Replace("/**where**/", "AND pa.\"CODIGO\" = @codigo");
    else
        query = query.Replace("/**where**/", "");

    using (var command = new NpgsqlCommand(query, connection))
    {
        if (!string.IsNullOrEmpty(codigo))
            command.Parameters.AddWithValue("@codigo", codigo);

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var articulo = new Articulo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO")),
                    Codigo = reader.GetString(reader.GetOrdinal("CodigoArticulo")),
                    Color = new Color
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID_COLOR")),
                        Codigo = reader.GetString(reader.GetOrdinal("CodigoColor"))
                    }
                };

                var consulta = new ConsultaTallerCorte
                {
                    articulo = articulo,
                    CantidadEnCorteUnitario = reader.IsDBNull(reader.GetOrdinal("CORTE"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("CORTE")),
                    CantidadEnTallerUnitario = 0,
                    CantidadSeparadoUnitario = 0,
                    StockUnitario = 0,
                    CantidadEstanteriaUnitario = 0
                };

                resultado.Add(consulta);
            }
        }
    }

    return resultado;
}


private List<ConsultaTallerCorte> consultarEnTaller(string? codigo, NpgsqlConnection connection)
{
    var resultado = new List<ConsultaTallerCorte>();

    string query = @"
        SELECT 
            a.""ID_ARTICULO"",
            pa.""CODIGO"" AS ""CodigoArticulo"",
            c.""ID_COLOR"",
            c.""CODIGO"" AS ""CodigoColor"",
            SUM(pa.""CANTIDAD"") AS ""TALLER""
        FROM PUBLIC.""PEDIDO_PRODUCCION"" pp
        JOIN PUBLIC.""PRODUCCION_ARTICULO"" pa ON pp.""ID_PEDIDO_PRODUCCION"" = pa.""ID_PEDIDO_PRODUCCION""
        JOIN PUBLIC.""ARTICULO"" a ON a.""ID_ARTICULO"" = pa.""ID_ARTICULO""
        JOIN PUBLIC.""COLOR"" c ON a.""ID_COLOR"" = c.""ID_COLOR""
        JOIN PUBLIC.""FABRICANTE"" f ON f.""ID_FABRICANTE"" = pp.""ID_FABRICANTE""
        WHERE pp.""ID_ESTADO_PEDIDO_PROD"" = 3
        AND f.""APORTA_STOCK"" = TRUE

        /**where**/
        GROUP BY a.""ID_ARTICULO"", pa.""CODIGO"", c.""ID_COLOR"", c.""CODIGO"", c.""ID_COLOR""
        ORDER BY a.""ID_ARTICULO"";";

    if (!string.IsNullOrEmpty(codigo))
        query = query.Replace("/**where**/", "AND pa.\"CODIGO\" = @codigo");
    else
        query = query.Replace("/**where**/", "");

    using (var command = new NpgsqlCommand(query, connection))
    {
        if (!string.IsNullOrEmpty(codigo))
            command.Parameters.AddWithValue("@codigo", codigo);

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var articulo = new Articulo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO")),
                    Codigo = reader.GetString(reader.GetOrdinal("CodigoArticulo")),
                    Color = new Color
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID_COLOR")),
                        Codigo = reader.GetString(reader.GetOrdinal("CodigoColor"))
                    }
                };

                var consulta = new ConsultaTallerCorte
                {
                    articulo = articulo,
                    CantidadEnCorteUnitario = 0,
                    CantidadEnTallerUnitario = reader.IsDBNull(reader.GetOrdinal("TALLER"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("TALLER")),
                    CantidadSeparadoUnitario = 0,
                    StockUnitario = 0,
                    CantidadEstanteriaUnitario = 0
                };

                resultado.Add(consulta);
            }
        }
    }

    return resultado;
}

private List<ConsultaTallerCorte> consultarSeparados(string? codigo, NpgsqlConnection connection)
{
    var resultado = new List<ConsultaTallerCorte>();

    string query = @"
        SELECT 
            a.""ID_ARTICULO"",
            ap.""CODIGO"" AS ""CodigoArticulo"",
            c.""ID_COLOR"",
            c.""CODIGO"" AS ""CodigoColor"",
            SUM(ap.""CANTIDAD"") AS ""SEPARADO""
        FROM PUBLIC.""PRESUPUESTO"" p
        JOIN PUBLIC.""ARTICULO_PRESUPUESTO"" ap ON p.""ID_PRESUPUESTO"" = ap.""ID_PRESUPUESTO""
        JOIN PUBLIC.""ARTICULO"" a ON a.""ID_ARTICULO"" = ap.""ID_ARTICULO""
        JOIN PUBLIC.""COLOR"" c ON a.""ID_COLOR"" = c.""ID_COLOR""
        WHERE p.""ID_ESTADO"" IN (2,3,4)
        AND ap.""HAY_STOCK"" = TRUE
        /**where**/
        GROUP BY a.""ID_ARTICULO"", ap.""CODIGO"", c.""ID_COLOR"", c.""CODIGO"", c.""ID_COLOR""
        ORDER BY a.""ID_ARTICULO"";";

    if (!string.IsNullOrEmpty(codigo))
        query = query.Replace("/**where**/", "AND ap.\"CODIGO\" = @codigo");
    else
        query = query.Replace("/**where**/", "");

    using (var command = new NpgsqlCommand(query, connection))
    {
        if (!string.IsNullOrEmpty(codigo))
            command.Parameters.AddWithValue("@codigo", codigo);

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var articulo = new Articulo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ARTICULO")),
                    Codigo = reader.GetString(reader.GetOrdinal("CodigoArticulo")),
                    Color = new Color
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID_COLOR")),
                        Codigo = reader.GetString(reader.GetOrdinal("CodigoColor"))
                    }
                };

                var consulta = new ConsultaTallerCorte
                {
                    articulo = articulo,
                    CantidadEnCorteUnitario = 0,
                    CantidadEnTallerUnitario = 0,
                    CantidadSeparadoUnitario = reader.IsDBNull(reader.GetOrdinal("SEPARADO"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("SEPARADO")),
                    StockUnitario = 0,
                    CantidadEstanteriaUnitario = 0
                };

                resultado.Add(consulta);
            }
        }
    }

    return resultado;
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

