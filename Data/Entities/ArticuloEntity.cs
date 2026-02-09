namespace BlumeAPI.Data.Entities;
public class ArticuloEntity
{
    public int IdArticulo { get; set; }

    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;

    public int IdColor { get; set; }
    public ColorEntity Color { get; set; } = null!;

    public int IdMedida { get; set; }
    public MedidaEntity Medida { get; set; } = null!;

    public int? IdSubFamilia { get; set; }
    public SubFamiliaEntity? SubFamilia { get; set; }

    public int? IdArticuloPrecio { get; set; }
    public ArticuloPrecioEntity? ArticuloPrecio { get; set; }

    public int IdFabricante { get; set; }

    public bool? Habilitado { get; set; }

    public int? Stock { get; set; }

    public int? IdAsociadoRelleno { get; set; }
}
