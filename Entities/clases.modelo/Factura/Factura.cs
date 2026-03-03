using System.Globalization;

public class Factura
{
    public static string TABLA = "FACTURA";

    public int Id { get; set; }

    public DateTime FechaFactura { get; set; }

    public Cliente Cliente { get; set; }

    public bool EximirIVA { get; set; }

    public List<ArticuloFactura>? Articulos { get; set; }

    public Presupuesto? Presupuesto { get; set; }

    public decimal? ImporteBruto { get; set; }

    public int? PuntoDeVenta { get; set; }

    public int? NumeroComprobante { get; set; }

    public long? CaeNumero { get; set; }

    public DateTime? FechaVencimientoCae { get; set; }

    public decimal? ImporteNeto { get; set; }

    public decimal? Iva { get; set; }

    public string TipoFactura { get; set; }

    public int? DescuentoGeneral { get; set; }

    // =====================================================
    // 🔹 MÉTODO CENTRAL DE CÁLCULO
    // =====================================================
    private (decimal Subtotal,
             decimal DescuentoTotal,
             decimal Iva,
             decimal Total) CalcularMontos()
    {
        if (Articulos == null || !Articulos.Any())
            return (0m, 0m, 0m, 0m);

        decimal totalBruto = 0m;
        decimal totalConDescItem = 0m;

        foreach (var articulo in Articulos)
        {
            decimal precioBase = articulo.PrecioUnitario * articulo.Cantidad;
            totalBruto += precioBase;

            decimal precioConDescItem =
                precioBase * (1 - (articulo.Descuento / 100m));

            totalConDescItem += precioConDescItem;
        }

        // 🔹 Descuento general
        decimal descuentoGeneralMonto = 0m;

        if (DescuentoGeneral.HasValue && DescuentoGeneral.Value > 0)
        {
            descuentoGeneralMonto =
                totalConDescItem * (DescuentoGeneral.Value / 100m);
        }

        decimal totalFinal = totalConDescItem - descuentoGeneralMonto;

        decimal subtotal;
        decimal iva;

        if (EximirIVA)
        {
            subtotal = totalFinal;
            iva = 0m;
        }
        else
        {
            subtotal = totalFinal / 1.21m;
            iva = totalFinal - subtotal;
        }

        decimal descuentoTotal = totalBruto - totalFinal;

        return (
            Redondear(subtotal),
            Redondear(descuentoTotal),
            Redondear(iva),
            Redondear(totalFinal)
        );
    }

    // =====================================================
    // 🔹 MÉTODOS PÚBLICOS USADOS EN PDF
    // =====================================================

    public decimal calcularSubtotal()
        => CalcularMontos().Subtotal;

    public decimal calcularTotal()
        => CalcularMontos().Total;

    public decimal calcularIva()
        => CalcularMontos().Iva;

    public decimal CalcularDescuento()
        => CalcularMontos().DescuentoTotal;

    // =====================================================
    // 🔹 REDONDEO AFIP CORRECTO
    // =====================================================
    private decimal Redondear(decimal valor)
    {
        return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
    }




public decimal CalcularSubtotalSinDescuentoGeneral()
{
    if (Articulos == null || !Articulos.Any())
        return 0m;

    decimal totalConDescItem = 0m;

    foreach (var articulo in Articulos)
    {
        decimal precioBase = articulo.PrecioUnitario * articulo.Cantidad;

        decimal precioConDescItem =
            precioBase * (1 - (articulo.Descuento / 100m));

        totalConDescItem += precioConDescItem;
    }

    decimal subtotal;
    subtotal = totalConDescItem;
    

    return Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);
}

}
