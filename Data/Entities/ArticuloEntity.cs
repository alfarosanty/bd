namespace BlumeAPI.Data.Entities;
public class ArticuloEntity
{
    public int IdArticulo { get; set; }
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;

    public int IdColor { get; set; }
    public int IdMedida { get; set; }
    public int? IdSubFamilia { get; set; }
    public int? IdArticuloPrecio { get; set; }


    public int IdFabricante { get; set; }

    public bool? Habilitado { get; set; }

    public int? Stock { get; set; }

}