namespace BlumeAPI.Entities.clases.modelo;

public class NotaDeCredito
{
    public int Id { get; set; }
    public DateTime FechaNota { get; set; }
    public int IdCliente { get; set; }
    public Cliente? Cliente { get; set; }
    public List<ArticuloNotaCredito>? Articulos { get; set; }
    public decimal? ImporteBruto { get; set; }
    public decimal? ImporteNeto { get; set; }
    public decimal? Iva { get; set; }
    public decimal? Descuento { get; set; }
    public string TipoNota { get; set; }
    public int? PuntoDeVenta { get; set; }
    public long? Cae { get; set; }
    public DateTime? FechaVencimientoCae { get; set; }
    public int? NumeroComprobante { get; set; }
    public int IdFacturaAsociada { get; set; }
    public string Motivo { get; set; }
}