using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeAPI.Models;

namespace BlumeApi.Models
{
    [Table("ARTICULO_INGRESO")]
    public class ArticuloIngreso
    {
        public static string TABLA = "ARTICULO_INGRESO";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_ARTICULO_INGRESO")]
        public int Id { get; set; }

        [Column("ID_ARTICULO")]
        [ForeignKey(nameof(Articulo))]
        public int IdArticulo { get; set; }
        public Articulo Articulo { get; set; } = null!;

        [Column("ID_INGRESO")]
        public int? IdIngreso { get; set; }

        [ForeignKey(nameof(IdIngreso))]
        public Ingreso? Ingreso { get; set; }

        [Column("CANTIDAD")]
        public int Cantidad { get; set; }

        [Column("CODIGO")]
        public string Codigo { get; set; } = string.Empty;

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
