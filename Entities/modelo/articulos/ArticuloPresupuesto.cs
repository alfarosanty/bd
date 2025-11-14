using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeApi.Models;


namespace BlumeAPI.Models{
    
[Table("ARTICULO_PRESUPUESTO")]
public class ArticuloPresupuesto
{
    public static string TABLA = "ARTICULO_PRESUPUESTO";

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_ARTICULO_PRESUPUESTO")]
    public int Id { get; set; }

    [Column("ID_ARTICULO")]
    [ForeignKey(nameof(Articulo))]
    public int IdArticulo { get; set; }

    public Articulo Articulo { get; set; } = null!;

    [Column("ID_PRESUPUESTO")]
    [ForeignKey(nameof(Presupuesto))]
    public int? IdPresupuesto { get; set; }

    public Presupuesto? Presupuesto { get; set; }

    [Column("CANTIDAD")]
    public int Cantidad { get; set; }

    [Column("CANTIDAD_PENDIENTE")]
    public int? CantidadPendiente { get; set; }

    [Column("PRECIO_UNITARIO", TypeName = "numeric(18,2)")]
    public decimal PrecioUnitario { get; set; }

    [Column("DESCUENTO", TypeName = "numeric(5,2)")]
    public decimal Descuento { get; set; }

    [Column("DESCRIPCION")]
    public string? Descripcion { get; set; }

    [Column("HAY_STOCK")]
    public bool HayStock { get; set; }

    [Column("CODIGO")]
    public string? Codigo { get; set; }
}

}