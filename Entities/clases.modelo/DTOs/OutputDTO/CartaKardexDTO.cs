using BlumeAPI.Entities.clases.modelo;

public class CartaKardexDTO
{
    public int IdArticulo { get; set; }
    public Articulo articulo{ get; set; }
    public string Codigo { get; set; }
    public string Descripcion { get; set; }

    public string Tipo { get; set; } // FACTURADO | INGRESADO

    public int DocumentoId { get; set; }
    public string DocumentoNombre { get; set; } // Cliente o Taller

    public DateTime Fecha { get; set; }
    public int Cantidad { get; set; }
}
