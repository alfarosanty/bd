namespace BlumeAPI.Data.Entities;

public class ClienteEntity
{
    public int Id { get; set; }
    public string RazonSocial { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Contacto { get; set; }
    public string? Domicilio { get; set; }
    public string? Localidad { get; set; }
    public string? Cuit { get; set; }
    public int IdCondicionFiscal { get; set; }
    public string? Provincia { get; set; }
    public string? Transporte { get; set; }
    public bool Valido { get; set; }
}