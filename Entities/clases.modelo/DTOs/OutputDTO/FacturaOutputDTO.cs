using System.Globalization;

public class FacturaOutputDTO
{
    public int Id { get; set; }
    public DateTime FechaFactura { get; set; }
    public Cliente Cliente { get; set; }
    public bool EximirIVA { get; set; }
    public List<ArticuloFacturaOutputDTO>? Articulos { get; set; }
    public int IdPresupuesto { get; set; }
    public decimal? ImporteBruto { get; set; }
    public int? PuntoDeVenta { get; set; }
    public int? NumeroComprobante { get; set; }
    public long? CaeNumero { get; set; }
    public DateTime? FechaVencimientoCae { get; set; }
    public decimal? ImporteNeto { get; set; }
    public decimal? Iva { get; set; }
    public string TipoFactura { get; set; }
    public int? DescuentoGeneral { get; set; }

}
