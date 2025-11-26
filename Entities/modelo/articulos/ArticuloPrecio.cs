using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace BlumeAPI.Models{

[Table("ARTICULO_PRECIO")]
public class ArticuloPrecio
{
    public static string TABLA = "ARTICULO_PRECIO";

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_ARTICULO_PRECIO")]
    public int Id { get; set; }

    [Column("CODIGO")]
    public string? Codigo { get; set; }

    [Column("DESCRIPCION")]
    public string? Descripcion { get; set; }

    [Column("PRECIO1", TypeName = "numeric(18,2)")]
    public decimal? Precio1 { get; set; }

    [Column("PRECIO2", TypeName = "numeric(18,2)")]
    public decimal? Precio2 { get; set; }

    [Column("PRECIO3", TypeName = "numeric(18,2)")]
    public decimal? Precio3 { get; set; }

    [Column("RELLENO")]
    public int? Relleno { get; set; }
}

}