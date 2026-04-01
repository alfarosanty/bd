namespace BlumeAPI.Entities
{
public class Ingreso
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    
    // Nueva propiedad de Estado
    public EstadoIngreso Estado { get; set; } = EstadoIngreso.Creado;

    public int IdTaller { get; set; }
    public Taller Taller { get; set; }
    public List<ArticuloIngreso> Articulos { get; set; } = new List<ArticuloIngreso>();
}
    
}
