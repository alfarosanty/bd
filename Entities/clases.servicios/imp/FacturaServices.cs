using System.Text;
using System.Xml;
using Npgsql;
using System.Globalization;


namespace BlumeAPI.Services;

public class FacturaServices
{



//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


private readonly HttpClient _httpClient;

    public FacturaServices()
    {
        _httpClient = new HttpClient();
    }
public async Task<AfipResponse> FacturarAsync(
    Factura factura,
    LoginTicketResponseData loginTicket,
    long cuitRepresentada)
{
    // 1️⃣ Crear el builder del comprobante y cliente AFIPs

    var datosFacturaAfip = new ComprobanteCaeBuilder();
    var afipClient = new AfipWsMtxcaClient("https://fwshomo.afip.gov.ar/wsmtxca/services/MTXCAService");



    // 2️⃣ Completar datos del comprobante

    int tipoFactura = factura.TipoFactura == "A" ? 1 : 6;
    var numeroComprobanteResponseXML = await afipClient.ConsultarUltimoAutorizadoAsync(loginTicket.Token, loginTicket.Sign, cuitRepresentada, tipoFactura, factura.PuntoDeVenta);
    UltimoComprobanteAutorizadoResult numeroComprobanteResponse = afipClient.ParseUltimoComprobanteAutorizadoResponse(numeroComprobanteResponseXML);

var numeroComprobante = numeroComprobanteResponse.Exitoso?(numeroComprobanteResponse.NumeroComprobante + 1 ?? 1) : 1;

    datosFacturaAfip.datosFactura(
        tipoFactura,
        factura.PuntoDeVenta,
        numeroComprobante,
        factura.FechaFactura
    );
    var tipoIVAReceptor = 0;
    if (factura.Cliente.CondicionFiscal.Codigo == "RI")
        tipoIVAReceptor = 1;
    else if (factura.Cliente.CondicionFiscal.Codigo == "MO")
        tipoIVAReceptor = 6;
    else
        tipoIVAReceptor = 5; // Consumidor Final
    datosFacturaAfip.Receptor(
        80, // CUIT
        long.Parse(factura.Cliente.Cuit.Replace("-", "")),
        tipoIVAReceptor // Responsable Inscripto
    );


var articulosAgrupados = AgruparPorCodigo(factura.Articulos!);

foreach (var articulos in articulosAgrupados.Values)
{
    var primero = articulos[0];

    decimal precioConIVA = primero.PrecioUnitario;
    decimal precioNeto = redondeoDecimales(precioConIVA / 1.21m);

    decimal descItemPct = primero.Descuento / 100m;
    decimal descGeneralPct = (factura.DescuentoGeneral ?? 0) / 100m;

    int cantidadTotal = articulos.Sum(a => a.Cantidad);

    decimal importeBonificacion;
    decimal importeIVA;
    decimal importeItem;
    decimal precioParaCalcular;
    int codigoCondicionIVA;

    if (tipoFactura == 1) // FACTURA A (neto + IVA)
    {
        decimal bonifItem = redondeoDecimales(precioNeto * descItemPct * cantidadTotal);

        decimal subtotalSinDescuento = precioNeto * cantidadTotal;
        decimal bonifGeneral = redondeoDecimales(subtotalSinDescuento * descGeneralPct);

        importeBonificacion = redondeoDecimales(bonifItem + bonifGeneral);

        decimal baseNeta = redondeoDecimales(subtotalSinDescuento - importeBonificacion);

        importeIVA = redondeoDecimales(baseNeta * 0.21m);
        importeItem = redondeoDecimales(baseNeta + importeIVA);

        precioParaCalcular = precioNeto;

        codigoCondicionIVA = 5;
    }
    else // FACTURA B (precios brutos)
    {
        decimal bonifItemBruto = redondeoDecimales(precioConIVA * descItemPct * cantidadTotal);

        decimal subtotalBruto = precioConIVA * cantidadTotal;
        decimal bonifGeneralBruto = redondeoDecimales(subtotalBruto * descGeneralPct);

        importeBonificacion = redondeoDecimales(bonifItemBruto + bonifGeneralBruto);

        decimal totalConDescuento = redondeoDecimales(subtotalBruto - importeBonificacion);






        /*PRUEBA IMPORTE IVA*/

        decimal bonifItem = redondeoDecimales(precioNeto * descItemPct * cantidadTotal);

        decimal subtotalSinDescuento = precioNeto * cantidadTotal;
        decimal bonifGeneral = redondeoDecimales(subtotalSinDescuento * descGeneralPct);

        importeBonificacion = redondeoDecimales(bonifItem + bonifGeneral);

        decimal baseNeta = redondeoDecimales(subtotalSinDescuento - importeBonificacion);

        importeIVA = redondeoDecimales(baseNeta * 0.21m);

        /*==================*/



        importeItem = totalConDescuento;

        precioParaCalcular = precioConIVA;

        codigoCondicionIVA = 5;
    }


var cantidadesPorColor = articulos
    .GroupBy(a => a.Articulo.Color.Codigo)
    .Select(g => new { Color = g.Key, Cant = g.Sum(a => a.Cantidad) })
    .OrderBy(x => x.Color) // opcional
    .ToList();

string detalleColores = string.Join(
    " ",
    cantidadesPorColor.Select(x => $"{x.Cant}{x.Color}")
);

string descripcionFinal = $"{primero.Descripcion} {detalleColores}";

    var item = new Item
    {
        unidadesMtx = 7,
        codigoMtx = primero.Codigo,
        codigo = primero.Codigo,
        descripcion = descripcionFinal,
        cantidad = cantidadTotal,
        codigoUnidadMedida = 7,

        precioUnitario = redondeoDecimales(precioParaCalcular),
        importeBonificacion = importeBonificacion,
        codigoCondicionIVA = codigoCondicionIVA,
        importeIVA = importeIVA,
        importeItem = importeItem
    };

    datosFacturaAfip.AgregarItem(item);
}




var totalItems = datosFacturaAfip.GetItems().Sum(i => i.importeItem);
var totalIVA   = datosFacturaAfip.GetItems().Sum(i => i.importeIVA);

decimal gravado;
decimal subtotal;
decimal total;

if (tipoFactura == 1) // A
{
    gravado = totalItems - totalIVA;
    subtotal = gravado;
    total = totalItems;
}
else // B
{
    total = totalItems;
    gravado = redondeoDecimales(total / 1.21m);
    subtotal = gravado;
    totalIVA = total - gravado;
}

datosFacturaAfip.Importes(
    redondeoDecimales(gravado),
    redondeoDecimales(subtotal),
    redondeoDecimales(total)
);



var subtotalIVA = new SubtotalIVA
{
    codigo = 5, // IVA 21%
    importe = datosFacturaAfip.GetItems().Sum(i => i.importeIVA)
};

    datosFacturaAfip.AgregarSubtotal(subtotalIVA);

// 4️⃣ Generar XML del comprobante
    string comprobanteXml = datosFacturaAfip.Build();


    // 5️⃣ Llamar al método AutorizarComprobanteAsync
    string responseXml = await afipClient.AutorizarComprobanteAsync(
        loginTicket.Token,
        loginTicket.Sign,
        cuitRepresentada,
        comprobanteXml
    );

    // 7️⃣ Retornar XML de respuesta (puedes parsearlo a objeto si quieres)

    var afip = afipClient.ParseAfipResponse(responseXml, factura.Id);
    

    if (!afip.Aprobado)
    {
        var errors = string.Join("; ", afip.Errores);
        throw new Exception($"Factura rechazada: {errors}");
    }

    if (afip.Observaciones.Any())
    {
        Console.WriteLine("Observaciones de AFIP:");
        foreach (var o in afip.Observaciones)
            Console.WriteLine(o);
    }



    return afip;
}

private decimal calcularPrecioUnitario(decimal precioFinal/*, decimal descuentoUnitario, decimal descuentoGeneral*/)
{
    return Math.Round(precioFinal / 1.21m, 2);
;
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
public  string getTabla()
    {
        return Presupuesto.TABLA;   
    }




    public  Presupuesto readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }

    private decimal redondeoDecimales(decimal valor)
{
    return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
}

public Dictionary<string, List<ArticuloFactura>> AgruparPorCodigo(List<ArticuloFactura> articulos)
{
    var mapa = new Dictionary<string, List<ArticuloFactura>>();

    foreach (var art in articulos)
    {
        if (!mapa.ContainsKey(art.Codigo))
        {
            mapa[art.Codigo] = new List<ArticuloFactura>();
        }

        mapa[art.Codigo].Add(art);
    }

    return mapa;
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
    cmd.Parameters.AddWithValue("NUMERO_DE_FACTURA", factura.NumeroComprobante ?? 0);
    cmd.Parameters.AddWithValue("CAE_NUMERO", (object?)factura.CaeNumero ?? DBNull.Value);
    cmd.Parameters.AddWithValue("FECHA_VENCIMIENTO_CAE", (object?)factura.FechaVencimientoCae ?? DBNull.Value);
    cmd.Parameters.AddWithValue("IMPORTE_NETO", factura.ImporteNeto);
    cmd.Parameters.AddWithValue("IVA", factura.Iva ?? 0);
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

public void ActualizarDatosAFIP(int facturaId, AfipResponse afip, Npgsql.NpgsqlConnection npgsqlConnection)
{
    string sql = @"
        UPDATE ""FACTURA""
        SET 
            ""NUMERO_FACTURA"" = @NUMERO_FACTURA,
            ""CAE_NUMERO"" = @CAE_NUMERO,
            ""FECHA_VENCIMIENTO_CAE"" = @FECHA_VENCIMIENTO_CAE
        WHERE ""ID_FACTURA"" = @ID_FACTURA
    ";

    using var cmd = new NpgsqlCommand(sql, npgsqlConnection);

    cmd.Parameters.AddWithValue("ID_FACTURA", facturaId);
    cmd.Parameters.AddWithValue("NUMERO_FACTURA", Convert.ToInt32(afip.numeroComprobante));

    if (!string.IsNullOrEmpty(afip.Cae))
        cmd.Parameters.AddWithValue("CAE_NUMERO", decimal.Parse(afip.Cae));
    else
        cmd.Parameters.AddWithValue("CAE_NUMERO", DBNull.Value);

    if (!string.IsNullOrEmpty(afip.CaeVencimiento))
        cmd.Parameters.AddWithValue("FECHA_VENCIMIENTO_CAE", DateTime.Parse(afip.CaeVencimiento));
    else
        cmd.Parameters.AddWithValue("FECHA_VENCIMIENTO_CAE", DBNull.Value);

    cmd.ExecuteNonQuery();
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
    
    int iva = (int)Math.Round((factura.ImporteBruto ?? 0) * 0.21m);
    factura.Iva = iva;
    factura.ImporteNeto = factura.ImporteBruto - iva;


    string sqlSeqNumero = "select nextval('\"NUMERO_FACTURA_seq\"')";
    using (var cmd = new NpgsqlCommand(sqlSeqNumero, npgsqlConnection))
    {
        int numeroFactura = Convert.ToInt32(cmd.ExecuteScalar());
        factura.NumeroComprobante = numeroFactura;
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
        NumeroComprobante = numeroFactura,
        CaeNumero = caeNumero,
        FechaVencimientoCae = fechaVencimientoCae,
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