namespace BlumeAPI.Data.Entities;

public class ArticuloIngresoEntity
{
    public int IdArticuloIngreso { get; set; }

    public int IdArticulo { get; set; }
    public int IdIngreso { get; set; }

    public string Codigo { get; set; }
    public string Descripcion { get; set; }

    public int Cantidad { get; set; }

    public DateTime FechaIngreso { get; set; }
    public DateTime FechaCreacion { get; set; } // auditoría
}
