using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlumeApi.Models
{
    [Table("SUBFAMILIA")]
    public class SubFamilia
    {
        public static string TABLA = "SUBFAMILIA";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_SUBFAMILIA")]
        public int? Id { get; set; }

        [Column("CODIGO")]
        public string? Codigo { get; set; }

        [Column("DESCRIPCION")]
        public string? Descripcion { get; set; }
    }
}
