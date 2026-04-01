namespace BlumeAPI.Entities;

public class ArticuloNotaCredito : IArticuloConStock
{
    public int Id { get; set; }
    public int IdNotaDeCredito { get; set; }
    public int IdArticulo { get; set; }
    public Articulo? Articulo { get; set; }
    public int Cantidad { get; set; }
    public string Codigo { get; set; }
    public string Descripcion { get; set; }
    public decimal Descuento { get; set; }

    // Calculados en runtime, no persistidos
    public decimal PrecioUnitario { get; set; }
    public decimal MontoBruto { get; set; }
    public decimal MontoNeto { get; set; }
    public decimal Iva { get; set; }
}
