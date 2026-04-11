namespace BlumeAPI.Entities;
public class DatosAfip
{
    public string Servicio { get; set; } = null!;
    public byte[] Certificado { get; set; } = null!;
    public string Contrasena { get; set; } = null!;
    public string UrlWsaa { get; set; } = null!;
}