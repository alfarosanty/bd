public class AutenticacionDTO
{
    public long Cuit { get; set; }
    public string PfxPath { get; set; }
    public string PfxPassword { get; set; }
    public string Servicio { get; set; } = "wsfe";
}