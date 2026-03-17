namespace BlumeAPI.Data.Entities;

public class ArticuloFacturaEntity
{
    public int IdArticuloFactura { get; set; }
    public int IdFactura { get; set; }
    public int IdArticulo { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal Descuento { get; set; }

    // 🔗 Relaciones
    // La referencia al artículo original (por si necesitás ver stock o talle/color)
    public virtual ArticuloEntity Articulo { get; set; } = null!;
}