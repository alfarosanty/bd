 public class ArticuloFacturaOutputDTO
{

    public int IdArticuloFactura { get; set; }
    public int IdFactura { get; set; }
    public Articulo Articulo { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public string Codigo { get; set; }
    public string Descripcion { get; set; }
    public decimal montoBruto { get; set; }
    public decimal montoNeto { get; set; }
    public decimal iva { get; set; }

    }