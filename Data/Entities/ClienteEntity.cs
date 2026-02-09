namespace BlumeAPI.Data.Entities;
public class ClienteEntity
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Direccion { get; set; } = null!;
}