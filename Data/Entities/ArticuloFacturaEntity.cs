namespace BlumeAPI.Data.Entities;
public class ArticuloFacturaEntity
{
    public int IdArticuloFactura { get; set; }

    public int IdArticulo { get; set; }
    public int IdFactura { get; set; }

    public string Codigo { get; set; }
    public string Descripcion { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }

    public DateTime FechaCreacion { get; set; } // auditoría
}
