using Npgsql;

namespace BlumeAPI.Services;

public class FacturaServices
{

public  string getTabla()
    {
        return Presupuesto.TABLA;   
    }




    public  Presupuesto readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }


    public int crear(Factura factura, Npgsql.NpgsqlConnection npgsqlConnection)
{

    if(factura.PuntoDeVenta == 0){
        completarDatosFactura(factura, npgsqlConnection);
    }
    // OBTENGO EL ID DE LA FACTURA
    string sqlSeq = "select nextval('\"FACTURA_ID_FACTURA_seq\"')";
    NpgsqlCommand cmdSeq = new NpgsqlCommand(sqlSeq, npgsqlConnection);
    Console.WriteLine("Ingreso el  " + Factura.TABLA + " el remito ingreso" + sqlSeq);
    int idFactura = Convert.ToInt32(cmdSeq.ExecuteScalar());

    // CREO EL INSERT EN LA TABLA PRESUPUESTO
    string sqlInsert = "INSERT INTO \"" + Factura.TABLA + "\" (\"ID_FACTURA\",\"ID_CLIENTE\", \"FECHA_FACTURA\", \"IMPORTE_BRUTO\", \"EXIMIR_IVA\", \"ID_PRESUPUESTO\", \"PUNTO_DE_VENTA\", \"NUMERO_FACTURA\", \"CAE_NUMERO\", \"FECHA_VENCIMIENTO_CAE\", \"IMPORTE_NETO\", \"IVA\", \"TIPO_FACTURA\", \"DESCUENTO\") " +
                       "VALUES(@FACTURA, @ID_CLIENTE, @FECHA_FACTURA, @IMPORTE_BRUTO, @EXIMIR_IVA, @ID_PRESUPUESTO, @PUNTO_DE_VENTA, @NUMERO_DE_FACTURA, @CAE_NUMERO, @FECHA_VENCIMIENTO_CAE, @IMPORTE_NETO, @IVA, @TIPO_FACTURA, @DESCUENTO)";
    NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
    cmd.Parameters.AddWithValue("FACTURA", idFactura);
    cmd.Parameters.AddWithValue("ID_CLIENTE", factura.Cliente.Id);
    cmd.Parameters.AddWithValue("FECHA_FACTURA", factura.FechaFactura);
    cmd.Parameters.AddWithValue("IMPORTE_BRUTO", factura.ImporteBruto);
    cmd.Parameters.AddWithValue("EXIMIR_IVA", factura.EximirIVA);
    cmd.Parameters.AddWithValue("ID_PRESUPUESTO", factura.Presupuesto?.Id ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("PUNTO_DE_VENTA", factura.PuntoDeVenta);
    cmd.Parameters.AddWithValue("NUMERO_DE_FACTURA", factura.NumeroFactura);
    cmd.Parameters.AddWithValue("CAE_NUMERO", (object?)factura.CaeNumero ?? DBNull.Value);
    cmd.Parameters.AddWithValue("FECHA_VENCIMIENTO_CAE", (object?)factura.FechaVencimiento ?? DBNull.Value);
    cmd.Parameters.AddWithValue("IMPORTE_NETO", factura.ImporteNeto);
    cmd.Parameters.AddWithValue("IVA", factura.Iva);
    cmd.Parameters.AddWithValue("TIPO_FACTURA", factura.TipoFactura);
    cmd.Parameters.AddWithValue("DESCUENTO", factura.DescuentoGeneral);
    cmd.ExecuteNonQuery();

    // RECORRO Y GUARDO LOS ARTICULOS EN LA TABLA ARTICULOPRESUPUESTO
    if (factura.Articulos != null)
    {
        foreach (ArticuloFactura ap in factura.Articulos)
        {
            string sqlArticuloInsert = "INSERT INTO \"" + ArticuloFactura.TABLA + "\" " +
                                       "(\"ID_ARTICULO\", \"ID_FACTURA\", \"CANTIDAD\", \"PRECIO_UNITARIO\", \"DESCUENTO\", \"DESCRIPCION\", \"CODIGO\") " +
                                       "VALUES (@ID_ARTICULO, @ID_FACTURA, @CANTIDAD, @PRECIO_UNITARIO, @DESCUENTO, @DESCRIPCION, @CODIGO)";
            NpgsqlCommand cmdArticulo = new NpgsqlCommand(sqlArticuloInsert, npgsqlConnection);
            cmdArticulo.Parameters.AddWithValue("ID_FACTURA", idFactura);
            cmdArticulo.Parameters.AddWithValue("ID_ARTICULO", ap.Articulo.Id);
            cmdArticulo.Parameters.AddWithValue("CANTIDAD", ap.Cantidad);
            cmdArticulo.Parameters.AddWithValue("PRECIO_UNITARIO", ap.PrecioUnitario);
            cmdArticulo.Parameters.AddWithValue("DESCRIPCION", ap.Descripcion);
            cmdArticulo.Parameters.AddWithValue("CODIGO", ap.Codigo);
            cmdArticulo.Parameters.AddWithValue("DESCUENTO", ap.Descuento);

            cmdArticulo.ExecuteNonQuery();

            Console.WriteLine("Ingreso el " + ArticuloPresupuesto.TABLA + " el artículo " + ap.Articulo.Id);

        }
                 actualizarStock(factura.Articulos, npgsqlConnection);
    }

    return idFactura;
}

public List<RespuestaEstadistica> facturacionXCliente(DateTime? fechaInicio, DateTime? fechaFin, NpgsqlConnection npgsqlConnection)
{
    List<RespuestaEstadistica> lista = new List<RespuestaEstadistica>();

    string sqlSelect = @"
SELECT 
    CLI.""RAZON_SOCIAL"",
    CLI.""ID_CLIENTE"", 
    SUM(DISTINCT (FA.""IMPORTE_BRUTO"" * (100 - COALESCE(FA.""DESCUENTO"", 0))/100)) AS MONTO_TOTAL,
    SUM(AF.""CANTIDAD"") AS CANTIDAD_TOTAL
FROM public.""FACTURA"" FA
JOIN public.""CLIENTE"" CLI ON FA.""ID_CLIENTE"" = CLI.""ID_CLIENTE""
JOIN public.""ARTICULO_FACTURA"" AF ON FA.""ID_FACTURA"" = AF.""ID_FACTURA""
WHERE FA.""FECHA_FACTURA"" BETWEEN @fechaInicio AND @fechaFin
GROUP BY CLI.""RAZON_SOCIAL"", CLI.""ID_CLIENTE""";

    using (var cmd = new NpgsqlCommand(sqlSelect, npgsqlConnection))
    {
        if (fechaInicio.HasValue && fechaFin.HasValue)
        {
            cmd.Parameters.AddWithValue("fechaInicio", fechaInicio.Value);
            cmd.Parameters.AddWithValue("fechaFin", fechaFin.Value);
        }

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var cliente = new Cliente
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_CLIENTE")),
                    RazonSocial = reader.GetString(reader.GetOrdinal("RAZON_SOCIAL"))
                };

                // MONTO_TOTAL viene como decimal
                decimal montoDecimal = reader.GetDecimal(reader.GetOrdinal("MONTO_TOTAL"));
                int monto = (int)montoDecimal;

                // CANTIDAD_TOTAL puede ser bigint
                long cantidadArticulosLong = reader.GetInt64(reader.GetOrdinal("CANTIDAD_TOTAL"));
                int cantidadArticulos = (int)cantidadArticulosLong;

                lista.Add(new RespuestaEstadistica
                {
                    Cliente = cliente,
                    Dinero = monto,
                    CantidadArticulos = cantidadArticulos
                });
            }
        }
    }

    return lista;
}

public List<Factura> getFacturaPorFiltro(int? idCliente, string? tipoFactura, int? puntoDeVenta, DateTime? fechaInicio, DateTime? fechaFin, NpgsqlConnection con)
{
    List<Factura> facturas = new List<Factura>();
    List<string> condiciones = new List<string>();
    var cmd = new NpgsqlCommand();

    string sql = $"SELECT * FROM \"{Factura.TABLA}\"";

    // Filtro por fecha: SIEMPRE presente
    condiciones.Add("\"FECHA_FACTURA\" >= @fechaInicio");
    condiciones.Add("\"FECHA_FACTURA\" <= @fechaFin");
    cmd.Parameters.AddWithValue("fechaInicio", fechaInicio!.Value);  // el ! indica que no es null
    cmd.Parameters.AddWithValue("fechaFin", fechaFin!.Value);

    // Agregamos condiciones según filtros presentes
    if (idCliente.HasValue)
    {
        condiciones.Add("\"ID_CLIENTE\" = @idCliente");
        cmd.Parameters.AddWithValue("idCliente", idCliente.Value);
    }
    if (!string.IsNullOrEmpty(tipoFactura))
    {
        condiciones.Add("\"TIPO_FACTURA\" = @tipoFactura");
        cmd.Parameters.AddWithValue("tipoFactura", tipoFactura);
    }
    if (puntoDeVenta.HasValue)
    {
        condiciones.Add("\"PUNTO_DE_VENTA\" = @puntoDeVenta");
        cmd.Parameters.AddWithValue("puntoDeVenta", puntoDeVenta.Value);
    }

    sql += " WHERE " + string.Join(" AND ", condiciones);
    sql += " ORDER BY \"FECHA_FACTURA\" DESC";

    cmd.CommandText = sql;
    cmd.Connection = con;

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        facturas.Add(ReadFactura(reader, con));
    }

    return facturas;
    
    }

/*public List<ArticuloFactura> getArticulosPorIdFactura(int idFactura, NpgsqlConnection con)
{
    List<ArticuloFactura> articulos = new List<ArticuloFactura>();

    string sql = $"SELECT * FROM \"{ArticuloFactura.TABLA}\" WHERE \"ID_FACTURA\" = @idFactura";

    using var cmd = new NpgsqlCommand(sql, con);
    cmd.Parameters.AddWithValue("idFactura", idFactura);

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        articulos.Add(ReadArticuloFactura(reader, con)); // ← le paso la conexión
    }

    return articulos;
}

*/



    private decimal calcularPrecioFinal(List<ArticuloFactura> articulosFacturas)
{
    decimal precioConDescuento = 0;

    foreach (var articuloFactura in articulosFacturas)
    {
        decimal descuento = articuloFactura.Descuento; // Asume 0 si es nulo
        decimal precioDescontado = articuloFactura.PrecioUnitario - (articuloFactura.PrecioUnitario * (descuento * 0.01m));

        precioConDescuento += precioDescontado;  // Todo es decimal ahora
    }

    return precioConDescuento;
}

private static int CalcularTotal(List<ArticuloFactura> articulos)
{
    if (articulos == null)
    {
        return 0;
    }

    decimal sumaTotal = articulos.Sum(articulo => articulo.PrecioUnitario * articulo.Cantidad);
    return Convert.ToInt32(Math.Round(sumaTotal, MidpointRounding.AwayFromZero));
}



private static void actualizarStock(List<ArticuloFactura> egresoArticulos, NpgsqlConnection conex)
{
    string updateQuery = @"
        UPDATE """ + Articulo.TABLA + @"""
        SET ""STOCK"" = COALESCE(""STOCK"", 0) - @CANTIDAD
        WHERE ""ID_ARTICULO"" = @ID_ARTICULO;
    ";

    using (var cmd = new NpgsqlCommand(updateQuery, conex))
    {
        foreach (var ea in egresoArticulos)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CANTIDAD", ea.Cantidad);
            cmd.Parameters.AddWithValue("@ID_ARTICULO", ea.Articulo.Id);

            cmd.ExecuteNonQuery();
        }
    }
}

private static void completarDatosFactura(Factura factura, Npgsql.NpgsqlConnection npgsqlConnection)
{
    factura.ImporteBruto = CalcularTotal(factura.Articulos);
    
    int iva = (int)Math.Round((factura.ImporteBruto ?? 0) * 0.21);
    factura.Iva = iva;
    factura.ImporteNeto = factura.ImporteBruto - iva;


    string sqlSeqNumero = "select nextval('\"NUMERO_FACTURA_seq\"')";
    using (var cmd = new NpgsqlCommand(sqlSeqNumero, npgsqlConnection))
    {
        int numeroFactura = Convert.ToInt32(cmd.ExecuteScalar());
        factura.NumeroFactura = numeroFactura;
    }
}

private static Factura ReadFactura(NpgsqlDataReader reader, NpgsqlConnection conex)
{
    int id = (int)reader["ID_FACTURA"];
    int idCliente = (int)reader["ID_CLIENTE"];
    DateTime fecha = (DateTime)reader["FECHA_FACTURA"];
    bool eximirIVA = (bool)reader["EXIMIR_IVA"];

    int? idPresupuesto = reader["ID_PRESUPUESTO"] != DBNull.Value ? Convert.ToInt32(reader["ID_PRESUPUESTO"]) : (int?)null;
    int? importeBruto = reader["IMPORTE_BRUTO"] != DBNull.Value ? Convert.ToInt32(reader["IMPORTE_BRUTO"]) : (int?)null;
    int? numeroFactura = reader["NUMERO_FACTURA"] != DBNull.Value ? Convert.ToInt32(reader["NUMERO_FACTURA"]) : (int?)null;
    int? caeNumero = reader["CAE_NUMERO"] != DBNull.Value ? Convert.ToInt32(reader["CAE_NUMERO"]) : (int?)null;
    DateTime? fechaVencimientoCae = reader["FECHA_VENCIMIENTO_CAE"] != DBNull.Value ? Convert.ToDateTime(reader["FECHA_VENCIMIENTO_CAE"]) : (DateTime?)null;
    int? importeNeto = reader["IMPORTE_NETO"] != DBNull.Value ? Convert.ToInt32(reader["IMPORTE_NETO"]) : (int?)null;
    int? iva = reader["IVA"] != DBNull.Value ? Convert.ToInt32(reader["IVA"]) : (int?)null;
    int puntoDeVenta = Convert.ToInt32(reader["PUNTO_DE_VENTA"]);
    string tipoFactura = reader["TIPO_FACTURA"].ToString();
    int? descuentoGeneral = reader["DESCUENTO"] != DBNull.Value ? Convert.ToInt32(reader["DESCUENTO"]) : (int?)null;

    // Usar la misma conexión para obtener Cliente y Presupuesto
    CConexion con = new CConexion();
    NpgsqlConnection npgsqlConnection2 = con.establecerConexion();
    Cliente cliente = new ClienteServices().GetCliente(idCliente, npgsqlConnection2);

    return new Factura
    {
        Id = id,
        FechaFactura = fecha,
        EximirIVA = eximirIVA,
        Cliente = cliente,
        Presupuesto = idPresupuesto.HasValue ? new Presupuesto { Id = idPresupuesto.Value } : null,
        ImporteBruto = importeBruto,
        NumeroFactura = numeroFactura,
        CaeNumero = caeNumero,
        FechaVencimiento = fechaVencimientoCae,
        ImporteNeto = importeNeto,
        Iva = iva,
        PuntoDeVenta = puntoDeVenta,
        TipoFactura = tipoFactura,
        DescuentoGeneral = descuentoGeneral
    };
}
/*
private static ArticuloFactura ReadArticuloFactura(NpgsqlDataReader reader, NpgsqlConnection conex)
{
    // Campos del artículo en la factura
    int idArticulo = Convert.ToInt32(reader["ID_ARTICULO"]);
    int idFactura = Convert.ToInt32(reader["ID_FACTURA"]);
    int cantidad = Convert.ToInt32(reader["CANTIDAD"]);
    decimal precioUnitario = Convert.ToDecimal(reader["PRECIO_UNITARIO"]);
    decimal descuento = Convert.ToDecimal(reader["DESCUENTO"]);
    string codigo = reader["CODIGO"].ToString();
    string descripcion = reader["DESCRIPCION"].ToString();

    // Obtener objetos completos (opcional, si querés que venga cargado)
    Articulo articulo = new ArticuloServices().GetArticulo(idArticulo, conex);
    Factura factura = {Id:idFactura};

    articuloFactura = new ArticuloFactura()

    
        articuloFactura.Articulo = articulo;
        articuloFactura.Factura = factura;
        articuloFactura.Cantidad = cantidad;
        articuloFactura.PrecioUnitario = precioUnitario;
        articuloFactura.Descuento = descuento;
        articuloFactura.Codigo = codigo;
        articuloFactura.Descripcion = descripcion
    
}



*/

}