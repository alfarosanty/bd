public class UltimoComprobanteAutorizadoResult
{
    public bool Exitoso { get; set; }
    public int? NumeroComprobante { get; set; }
    public List<AfipError> Errores { get; set; } = new();
}

public class AfipError
{
    public int Codigo { get; set; }
    public string Descripcion { get; set; }
}
