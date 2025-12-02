public class AfipResponse
{
    public bool Aprobado { get; set; }
    public long idFactura { get; set; }
    public string numeroComprobante { get; set; }
    public string Cae { get; set; }
    public string CaeVencimiento { get; set; }
    public List<string> Errores { get; set; } = new();
    public List<string> Observaciones { get; set; } = new();
}
