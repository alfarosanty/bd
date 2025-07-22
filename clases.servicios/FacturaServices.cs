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
    string sqlInsert = "INSERT INTO \"" + Factura.TABLA + "\" (\"ID_FACTURA\",\"ID_CLIENTE\", \"FECHA_FACTURA\", \"IMPORTE_BRUTO\", \"EXIMIR_IVA\", \"ID_PRESUPUESTO\", \"PUNTO_DE_VENTA\", \"NUMERO_FACTURA\", \"CAE_NUMERO\", \"FECHA_VENCIMIENTO_CAE\", \"IMPORTE_NETO\", \"IVA\", \"TIPO_FACTURA\") " +
                       "VALUES(@FACTURA, @ID_CLIENTE, @FECHA_FACTURA, @IMPORTE_BRUTO, @EXIMIR_IVA, @ID_PRESUPUESTO, @PUNTO_DE_VENTA, @NUMERO_DE_FACTURA, @CAE_NUMERO, @FECHA_VENCIMIENTO_CAE, @IMPORTE_NETO, @IVA, @TIPO_FACTURA)";
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
  c.""ID_CLIENTE"", 
  c.""RAZON_SOCIAL"",
  COALESCE(facturas.MONTO_TOTAL, 0) AS MONTO_TOTAL,
  COALESCE(articulos.CANTIDAD_TOTAL, 0) AS CANTIDAD_TOTAL
FROM """ + Cliente.TABLA + @""" c
LEFT JOIN (
    SELECT ""ID_CLIENTE"", SUM(""IMPORTE_BRUTO"") AS MONTO_TOTAL
    FROM """ + Factura.TABLA + @"""
    GROUP BY ""ID_CLIENTE""
) facturas ON c.""ID_CLIENTE"" = facturas.""ID_CLIENTE""
LEFT JOIN (
    SELECT f.""ID_CLIENTE"", SUM(af.""CANTIDAD"") AS CANTIDAD_TOTAL
    FROM """ + Factura.TABLA + @""" f
    JOIN """ + ArticuloFactura.TABLA + @""" af ON f.""ID_FACTURA"" = af.""ID_FACTURA""
    GROUP BY f.""ID_CLIENTE""
) articulos ON c.""ID_CLIENTE"" = articulos.""ID_CLIENTE""";

string whereClause = "";
if (fechaInicio.HasValue && fechaFin.HasValue)
{
    whereClause = @" WHERE EXISTS (
        SELECT 1 FROM """ + Factura.TABLA + @""" f2 
        WHERE f2.""ID_CLIENTE"" = c.""ID_CLIENTE""
        AND f2.""FECHA_FACTURA"" BETWEEN @fechaInicio AND @fechaFin
    )";
}
string finalQuery = sqlSelect + whereClause;


    using (var cmd = new NpgsqlCommand(finalQuery, npgsqlConnection))
    {
        if (fechaInicio.HasValue && fechaFin.HasValue)
        {
            cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio.Value);
            cmd.Parameters.AddWithValue("@fechaFin", fechaFin.Value);
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
            var montoDecimal = reader.GetDecimal(reader.GetOrdinal("MONTO_TOTAL"));
            int monto = (int)montoDecimal;

            int cantidadArticulos = reader.GetInt32(reader.GetOrdinal("CANTIDAD_TOTAL"));


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




}