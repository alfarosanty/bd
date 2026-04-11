namespace BlumeAPI.Entities;

public class DatosAutenticacion
{
    public string UniqueId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string Firma { get; set; } = null!;
    public DateTime Expiracion { get; set; }
}