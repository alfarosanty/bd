namespace BlumeAPI.Entities
{
public class ArticuloIngreso : IArticuloConStock
{
    public static string TABLA = "ARTICULO_INGRESO";

    public int Id { get; set; }

    public int? IdIngreso { get; set; }
    public Ingreso? Ingreso { get; set; }
    
    public int IdArticulo { get; set; }
    public Articulo? Articulo { get; set; }

    public int Cantidad { get; set; }
    public string Codigo { get; set; }
    public string Descripcion { get; set; }
    public DateTime Fecha { get; set; }
}
    
}

