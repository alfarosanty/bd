using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeApi.Models;

[Table("INGRESO")]
public class Ingreso
{
    public static string TABLA = "INGRESO";

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_INGRESO")]
    public int Id { get; set; }

    [Column("FECHA")]
    public DateTime Fecha { get; set; }

    [Column("ID_TALLER")]
    public int IdTaller { get; set; }
    public Taller Taller { get; set; } = null!;

    [InverseProperty("Ingreso")]
    public List<ArticuloIngreso> Articulos { get; set; } = new();
}
