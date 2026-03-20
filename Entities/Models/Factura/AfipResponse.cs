public class AfipResponse
{
    public bool Aprobado { get; set; }
    public long idDocumento { get; set; }
    public string numeroComprobante { get; set; }
    public string Cae { get; set; }
    public DateTime? CaeVencimiento { get; set; }
    public List<string> Errores { get; set; } = new();
    public List<string> Observaciones { get; set; } = new();
}
