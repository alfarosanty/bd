namespace BlumeAPI.Entities;

public class DatosAutenticacion
{
    public int Id { get; set; }
    public string UniqueId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string Firma { get; set; } = null!;
    public DateTime Expiracion { get; set; }
    public string Servicio { get; set; } = null!;
}