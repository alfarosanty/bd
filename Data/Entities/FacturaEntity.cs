namespace BlumeAPI.Data.Entities;
public class FacturaEntity
{
    public int IdFactura { get; set; }
    public int IdCliente { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    // Navegación
    public ClienteEntity Cliente { get; set; } = null!;
    public List<ArticuloFacturaEntity> ArticuloFacturas { get; set; } = new List<ArticuloFacturaEntity>();
}