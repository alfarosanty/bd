using BlumeAPI;
using Npgsql;

public class PresupuestoServices
{
    public  string getTabla()
    {
        return Presupuesto.TABLA;
    }




    public  Presupuesto readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();

    }


public Presupuesto GetPresupuesto(int id, NpgsqlConnection conex) {
    try {
        Presupuesto presupuesto = null;
        string commandText = getSelect() + GetFromText() + " WHERE PR.\"ID_PRESUPUESTO\" = @id";

        using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex)) {
            Console.WriteLine("Consulta: " + commandText);
            cmd.Parameters.AddWithValue("id", id);

            using (NpgsqlDataReader reader = cmd.ExecuteReader()) {
                if (reader.Read()) {
                    presupuesto = ReadPresupeusto(reader, conex);
                }
            }
        }

        if (presupuesto == null) {
            throw new Exception($"No se encontró presupuesto con id {id}");
        }

        presupuesto.Articulos = getArticuloPresupuesto(presupuesto, conex);
        return presupuesto;
    }
    catch (Exception ex) {
        // Aquí podés lanzar una excepción más específica o simplemente relanzar la actual
        throw new Exception($"{ex.Message}", ex);
    }
}







     public List<Presupuesto> GetPresupuestoByCliente(int idCliente, NpgsqlConnection conex ){
            List<Presupuesto> presupuestos = new List<Presupuesto>();
            string commandText =  getSelect() + GetFromText()+ " WHERE PR.\"ID_CLIENTE\" = @id";
            using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", idCliente);
                     
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            presupuestos.Add(ReadPresupeusto(reader, conex));
                            
                            
                        }
                }
                return presupuestos;
        }
    
public List<EstadoPresupuesto> getEstadosPresupuesto(NpgsqlConnection conex)
{
    if (conex == null)
        throw new ArgumentNullException(nameof(conex), "La conexión no puede ser null.");

    var estadosPresupuesto = new List<EstadoPresupuesto>();

    string query = "SELECT \"ID_ESTADO\", \"CODIGO\", \"DESCRIPCION\" FROM \"ESTADO_PRESUPUESTO\" ORDER BY \"ID_ESTADO\"";

    using (var cmd = new NpgsqlCommand(query, conex))
    {
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var estado = new EstadoPresupuesto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ESTADO")),
                    Codigo = reader.IsDBNull(reader.GetOrdinal("CODIGO")) ? null : reader.GetString(reader.GetOrdinal("CODIGO")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("DESCRIPCION")) ? null : reader.GetString(reader.GetOrdinal("DESCRIPCION"))
                };
                estadosPresupuesto.Add(estado);
            }
        }
    }

    return estadosPresupuesto;
}


    
public int crear(Presupuesto presupuesto, Npgsql.NpgsqlConnection npgsqlConnection)
{
    // OBTENGO EL ID DEL PRESUPUESTO
    string sqlSeq = "select nextval('\"PRESUPUESTO_ID_PRESUPUESTO_seq\"')";
    NpgsqlCommand cmdSeq = new NpgsqlCommand(sqlSeq, npgsqlConnection);
    Console.WriteLine("Ingreso el  " + Presupuesto.TABLA + " el remito ingreso" + sqlSeq);
    int idPresupuesto = Convert.ToInt32(cmdSeq.ExecuteScalar());

    // CREO EL INSERT EN LA TABLA PRESUPUESTO
    string sqlInsert = "INSERT INTO \"" + Presupuesto.TABLA + "\" (\"ID_PRESUPUESTO\", \"FECHA_PRESUPUESTO\", \"ID_CLIENTE\", \"EXMIR_IVA\", \"ID_ESTADO\", \"DESCUENTO\", \"TOTAL_PRESUPUESTO\") " +
                       "VALUES(@ID_PRESUPUESTO, @FECHA_PRESUPUESTO, @ID_CLIENTE, @EXMIR_IVA, @ID_ESTADO, @DESCUENTO , @TOTAL_PRESUPUESTO)";
    NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
    cmd.Parameters.AddWithValue("ID_PRESUPUESTO", idPresupuesto);
    cmd.Parameters.AddWithValue("FECHA_PRESUPUESTO", presupuesto.Fecha);
    cmd.Parameters.AddWithValue("ID_CLIENTE", presupuesto.Cliente.Id);
    cmd.Parameters.AddWithValue("EXMIR_IVA", presupuesto.EximirIVA);
    cmd.Parameters.AddWithValue("ID_ESTADO", 1);
    cmd.Parameters.AddWithValue("DESCUENTO", presupuesto.descuentoGeneral);
    cmd.Parameters.AddWithValue("TOTAL_PRESUPUESTO", calcularTotal(presupuesto.Articulos));
    cmd.ExecuteNonQuery();

    // RECORRO Y GUARDO LOS ARTICULOS EN LA TABLA ARTICULOPRESUPUESTO
    if (presupuesto.Articulos != null)
    {
        foreach (ArticuloPresupuesto ap in presupuesto.Articulos)
        {
            string sqlArticuloInsert = "INSERT INTO \"" + ArticuloPresupuesto.TABLA + "\" " +
                                       "(\"ID_ARTICULO\", \"ID_PRESUPUESTO\", \"CANTIDAD\", \"PRECIO_UNITARIO\", \"DESCUENTO\", \"PENDIENTE\", \"DESCRIPCION\", \"CODIGO\") " +
                                       "VALUES (@ID_ARTICULO, @ID_PRESUPUESTO, @CANTIDAD, @PRECIO_UNITARIO, @DESCUENTO, @PENDIENTE, @DESCRIPCION, @CODIGO)";
            NpgsqlCommand cmdArticulo = new NpgsqlCommand(sqlArticuloInsert, npgsqlConnection);
            cmdArticulo.Parameters.AddWithValue("ID_PRESUPUESTO", idPresupuesto);
            cmdArticulo.Parameters.AddWithValue("ID_ARTICULO", ap.Articulo.Id);
            cmdArticulo.Parameters.AddWithValue("CANTIDAD", ap.cantidad);
            cmdArticulo.Parameters.AddWithValue("PRECIO_UNITARIO", ap.PrecioUnitario);
            cmdArticulo.Parameters.AddWithValue("DESCUENTO", ap.Descuento);
            cmdArticulo.Parameters.AddWithValue("PENDIENTE", ap.CantidadPendiente);
            cmdArticulo.Parameters.AddWithValue("DESCRIPCION", ap.descripcion);
            cmdArticulo.Parameters.AddWithValue("CODIGO", ap.codigo);

            cmdArticulo.ExecuteNonQuery();

            Console.WriteLine("Ingreso el " + ArticuloPresupuesto.TABLA + " el artículo " + ap.Articulo.Id);

        }
    }

    return idPresupuesto;
}


public int actualizar(Presupuesto presupuesto, Npgsql.NpgsqlConnection npgsqlConnection)
{
    if (presupuesto == null)
        throw new ArgumentNullException(nameof(presupuesto));

    if (npgsqlConnection == null)
        throw new ArgumentNullException(nameof(npgsqlConnection));

    using (var transaction = npgsqlConnection.BeginTransaction())
    {
        try
        {
            // Delete artículos antiguos
            string sqlDelete = "DELETE FROM \"" + ArticuloPresupuesto.TABLA + "\" WHERE \"ID_PRESUPUESTO\" = @ID_PRESUPUESTO";
            using (var cmdDelete = new NpgsqlCommand(sqlDelete, npgsqlConnection, transaction))
            {
                cmdDelete.Parameters.AddWithValue("ID_PRESUPUESTO", presupuesto.Id);
                cmdDelete.ExecuteNonQuery();
            }

            // Insert artículos nuevos
            if (presupuesto.Articulos != null)
            {
                foreach (var ap in presupuesto.Articulos)
                {
                    string sqlInsert = "INSERT INTO \"" + ArticuloPresupuesto.TABLA + "\" " +
                                       "(\"ID_ARTICULO\", \"ID_PRESUPUESTO\", \"CANTIDAD\", \"PENDIENTE\" , \"PRECIO_UNITARIO\",\"DESCUENTO\" , \"HAY_STOCK\", \"DESCRIPCION\", \"CODIGO\") " +
                                       "VALUES(@ID_ARTICULO, @ID_PRESUPUESTO, @CANTIDAD, @PENDIENTE, @PRECIO_UNITARIO, @DESCUENTO, @HAY_STOCK, @DESCRIPCION, @CODIGO)";
                    using (var cmdInsert = new NpgsqlCommand(sqlInsert, npgsqlConnection, transaction))
                    {
                        cmdInsert.Parameters.AddWithValue("ID_PRESUPUESTO", presupuesto.Id);
                        cmdInsert.Parameters.AddWithValue("ID_ARTICULO", ap.Articulo.Id);
                        cmdInsert.Parameters.AddWithValue("CANTIDAD", ap.cantidad);
                        cmdInsert.Parameters.AddWithValue("PENDIENTE", ap.CantidadPendiente);
                        cmdInsert.Parameters.AddWithValue("PRECIO_UNITARIO", ap.PrecioUnitario);
                        cmdInsert.Parameters.AddWithValue("DESCUENTO", ap.Descuento);
                        cmdInsert.Parameters.AddWithValue("HAY_STOCK", ap.hayStock);
                        cmdInsert.Parameters.AddWithValue("DESCRIPCION", ap.descripcion);
                        cmdInsert.Parameters.AddWithValue("CODIGO", ap.codigo);
                        cmdInsert.ExecuteNonQuery();
                    }
                }
            }

            // Actualizar total y estado
            string sqlUpdateTotal = "UPDATE \"" + Presupuesto.TABLA + "\" " +
                                    "SET \"TOTAL_PRESUPUESTO\" = @TOTAL_PRESUPUESTO, " +
                                    "\"FECHA_PRESUPUESTO\" = @FECHA_PRESUPUESTO, " +
                                    "\"ID_ESTADO\" = @ID_ESTADO, " +
                                    "\"DESCUENTO\" = @DESCUENTO " +
                                    "WHERE \"ID_PRESUPUESTO\" = @ID_PRESUPUESTO";
            using (var cmdUpdateTotal = new NpgsqlCommand(sqlUpdateTotal, npgsqlConnection, transaction))
            {
                cmdUpdateTotal.Parameters.AddWithValue("TOTAL_PRESUPUESTO", calcularTotal(presupuesto.Articulos));
                cmdUpdateTotal.Parameters.AddWithValue("ID_PRESUPUESTO", presupuesto.Id);
                cmdUpdateTotal.Parameters.AddWithValue("FECHA_PRESUPUESTO", presupuesto.Fecha);
                cmdUpdateTotal.Parameters.AddWithValue("ID_ESTADO", presupuesto.EstadoPresupuesto.Id);
                cmdUpdateTotal.Parameters.AddWithValue("DESCUENTO", presupuesto.descuentoGeneral ?? 0);
                cmdUpdateTotal.ExecuteNonQuery();
            }

            transaction.Commit();
            return presupuesto.Id;
        }
        catch (Exception ex)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // opcional: loggear fallo en rollback, no re-lanzar para no esconder la excepción original
            }

            // Re-lanzar la excepción para que la maneje la capa superior
            throw new Exception("Error al actualizar presupuesto: " + ex.Message, ex);
        }
    }
}


public List<Presupuesto> GetPresupuestosByIds(List<int> idsPresupuestos, NpgsqlConnection conex)
{
    var presupuestos = new List<Presupuesto>();

    foreach (var id in idsPresupuestos)
    {
        var presupuesto = GetPresupuesto(id, conex); // tu método que devuelve un Ingreso
        if (presupuesto != null)
        {
            presupuestos.Add(presupuesto);
        }
    }

    return presupuestos;
}



private static Presupuesto ReadPresupeusto(NpgsqlDataReader reader,NpgsqlConnection conex){
       int id =(int) reader["ID_PRESUPUESTO"];
       DateTime fecha = (DateTime) reader["FECHA_PRESUPUESTO"];
       bool eximirIVA = (bool)reader["EXMIR_IVA"];
       int idCliente =(int) reader["ID_CLIENTE"];
       int? descGeneral = reader["DESCUENTO"] != DBNull.Value ? (int?)reader["DESCUENTO"] : null;
       int? idEstadoPresupuesto = reader["ID_ESTADO"] != DBNull.Value ? (int?)reader["ID_ESTADO"] : null;

        EstadoPresupuesto estadoPresupuesto = null;

        if (idEstadoPresupuesto.HasValue)
        {
            estadoPresupuesto = new EstadoPresupuesto
            {
                Id = idEstadoPresupuesto.Value
            };
        }

       Presupuesto presupuesto = new Presupuesto{
             Id = id,
             Fecha = fecha,
             EximirIVA = eximirIVA,
             descuentoGeneral = descGeneral,
             EstadoPresupuesto = estadoPresupuesto,
             };

           CConexion cconexio =  new CConexion();
          NpgsqlConnection conex2= cconexio.establecerConexion();

           presupuesto.Cliente = new ClienteServices().GetCliente(idCliente, conex2);
          
           cconexio.cerrarConexion(conex2);
           return presupuesto;


}
    private static string getSelect()
        {
            return  $"SELECT PR.* ";
            
        }
    private static string getSelectByArticulo()
    {
        return  $"SELECT PR.* ,AP.\"CANTIDAD\" AS CANTIDAD ";
        
    }
    private static string GetFromText()
    {
        
        return "FROM \"PRESUPUESTO\" PR";
    }

private static string GetFromTextByArticulo()
    {
        
        return "FROM \"PRESUPUESTO\" PR,\"ARTICULO_PRESUPUESTO\" AP ";
    }

   private static string GetWhereTextByArticulo()
    {
        return "WHERE AP.\"ID_PRESUPUESTO\" = AP.\"ID_PRESUPUESTO\" ";
    }


    private static List<ArticuloPresupuesto> getArticuloPresupuesto(Presupuesto presupuesto,NpgsqlConnection conex ){
        List<ArticuloPresupuesto> articuloPresupuesto = new List<ArticuloPresupuesto>();
    string commandText = "SELECT AP.* FROM \"ARTICULO_PRESUPUESTO\" AP " +
                         "WHERE AP.\"ID_PRESUPUESTO\" = @IDPRESUPUESTO " +
                         "ORDER BY AP.\"ID_ARTICULO_PRESUPUESTO\""; 
                using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("IDPRESUPUESTO", presupuesto.Id);
                     
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            articuloPresupuesto.Add(ReadArticuloPresupeusto(reader, presupuesto,conex));
                            
                            
                        }
                }
                return articuloPresupuesto;
    }

    private static ArticuloPresupuesto ReadArticuloPresupeusto(NpgsqlDataReader reader, Presupuesto presupuesto, NpgsqlConnection conex)
{
    int idArticulo = (int)reader["ID_ARTICULO"];
    decimal precioUnitario = (decimal)reader["PRECIO_UNITARIO"];
    int cantidadAP = (int)reader["CANTIDAD"];
    decimal descuento = (decimal)reader["DESCUENTO"];

    bool hayStock = !reader.IsDBNull(reader.GetOrdinal("HAY_STOCK")) && (bool)reader["HAY_STOCK"];
    int pendiente = !reader.IsDBNull(reader.GetOrdinal("PENDIENTE")) ? (int)reader["PENDIENTE"] : 0;
    string desc = !reader.IsDBNull(reader.GetOrdinal("DESCRIPCION")) ? (string)reader["DESCRIPCION"] : " ";
    string cod = !reader.IsDBNull(reader.GetOrdinal("CODIGO")) ? (string)reader["CODIGO"] : " ";



    CConexion cConexion = new CConexion();
    NpgsqlConnection conex2 = cConexion.establecerConexion();
    Articulo articulo = new ArticuloServices().GetArticulo(idArticulo, conex2);
    cConexion.cerrarConexion(conex2);

    return new ArticuloPresupuesto
    {
        Articulo = articulo,
        PrecioUnitario = precioUnitario,
        cantidad = cantidadAP,
        Descuento = descuento,
        hayStock = hayStock,
        CantidadPendiente = pendiente,
        descripcion = desc,
        codigo = cod
    };  
}



private static int calcularTotal(List<ArticuloPresupuesto> articulos)
{
    if (articulos == null)
    {
        return 0;
    }
    decimal sumaTotal = articulos.Sum(articulo => articulo.PrecioUnitario * articulo.cantidad);
    int sumaTotalRedondeada = (int)Math.Round(sumaTotal);
    return sumaTotalRedondeada;
}


public List<ArticuloPresupuesto> articulosPresupuestados(
    int idPrecioArticulo, 
    DateTime? fechaInicio, 
    DateTime? fechaFin, 
    NpgsqlConnection npgsqlConnection)
{
    List<ArticuloPresupuesto> lista = new List<ArticuloPresupuesto>();

    string sqlSelect = @"
        SELECT 
            a.""CODIGO"",
            a.""DESCRIPCION"" AS articuloDescripcion,
            a.""ID_ARTICULO"",
            a.""ID_COLOR"",
            cl.""DESCRIPCION"" AS colorDescripcion,
            cl.""HEXA"" AS colorHexa,
            SUM(ap.""CANTIDAD"") AS cantidad_total
        FROM public.""ARTICULO_PRESUPUESTO"" ap
        JOIN public.""ARTICULO"" a ON ap.""ID_ARTICULO"" = a.""ID_ARTICULO""
        JOIN public.""COLOR"" cl ON cl.""ID_COLOR"" = a.""ID_COLOR""
        JOIN public.""PRESUPUESTO"" pr ON pr.""ID_PRESUPUESTO"" = ap.""ID_PRESUPUESTO""
        WHERE a.""ID_ARTICULO_PRECIO"" = @idArticuloPrecio
          AND (@fechaInicio IS NULL OR pr.""FECHA_PRESUPUESTO"" >= @fechaInicio)
          AND (@fechaFin IS NULL OR pr.""FECHA_PRESUPUESTO"" <= @fechaFin)
        GROUP BY a.""CODIGO"", a.""DESCRIPCION"", a.""ID_ARTICULO"", a.""ID_COLOR"", cl.""DESCRIPCION"", cl.""HEXA""
        ORDER BY cantidad_total DESC;
    ";

    using (var cmd = new NpgsqlCommand(sqlSelect, npgsqlConnection))
    {
        cmd.Parameters.AddWithValue("idArticuloPrecio", idPrecioArticulo);
        cmd.Parameters.AddWithValue("fechaInicio", fechaInicio.HasValue ? (object)fechaInicio.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("fechaFin", fechaFin.HasValue ? (object)fechaFin.Value : DBNull.Value);

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var articulo = new Articulo
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("ID_ARTICULO")) 
                            ? 0 
                            : reader.GetInt32(reader.GetOrdinal("ID_ARTICULO")),
                    Codigo = reader.IsDBNull(reader.GetOrdinal("CODIGO")) 
                            ? null 
                            : reader.GetString(reader.GetOrdinal("CODIGO")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("articuloDescripcion")) 
                            ? null 
                            : reader.GetString(reader.GetOrdinal("articuloDescripcion")),
                    articuloPrecio = new ArticuloPrecio { Id = idPrecioArticulo },
                    Color = new Color
                    {
                        Id = reader.IsDBNull(reader.GetOrdinal("ID_COLOR")) 
                                ? 0 
                                : reader.GetInt32(reader.GetOrdinal("ID_COLOR")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("colorDescripcion")) 
                                ? null 
                                : reader.GetString(reader.GetOrdinal("colorDescripcion")),
                        ColorHexa = reader.IsDBNull(reader.GetOrdinal("colorHexa")) 
                                ? null 
                                : reader.GetString(reader.GetOrdinal("colorHexa"))
                    }
                };

                var presuArt = new ArticuloPresupuesto
                {
                    Articulo = articulo,
                    cantidad = reader.IsDBNull(reader.GetOrdinal("cantidad_total")) 
                                ? 0 
                                : reader.GetInt32(reader.GetOrdinal("cantidad_total")),
                    codigo = reader.IsDBNull(reader.GetOrdinal("CODIGO")) 
                            ? null 
                            : reader.GetString(reader.GetOrdinal("CODIGO")),
                    descripcion = reader.IsDBNull(reader.GetOrdinal("articuloDescripcion")) 
                            ? null 
                            : reader.GetString(reader.GetOrdinal("articuloDescripcion"))
                };

                lista.Add(presuArt);
            }
        }
    }

    return lista;
}



}