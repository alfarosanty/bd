namespace BlumeAPI.Data.Entities;

public class FacturaEntity
{
    public int IdFactura { get; set; }
    public int IdCliente { get; set; }
    public DateTime FechaFactura { get; set; }
    public decimal ImporteBruto { get; set; }
    public bool EximirIva { get; set; }
    public int? IdPresupuesto { get; set; }
    public int PuntoDeVenta { get; set; }
    public int NumeroFactura { get; set; }
    public long? CaeNumero { get; set; }
    public DateTime? FechaVencimientoCae { get; set; }
    public decimal ImporteNeto { get; set; }
    public decimal Iva { get; set; }
    public string TipoFactura { get; set; } = null!;
    public decimal Descuento { get; set; }

    // 🔗 Relaciones
    public virtual ClienteEntity Cliente { get; set; } = null!;
    
    // Esta es la lista que vamos a recorrer en Angular para la NC
    public virtual ICollection<ArticuloFacturaEntity> ArticulosFactura { get; set; } = new List<ArticuloFacturaEntity>();
}