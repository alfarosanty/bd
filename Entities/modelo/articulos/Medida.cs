using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlumeApi.Models
{
    [Table("MEDIDA")]
    public class Medida
    {
        public static string TABLA = "MEDIDA";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_MEDIDA")]
        public int Id { get; set; }

        [Column("CODIGO")]
        [Required]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Column("DESCRIPCION")]
        [Required]
        [StringLength(255)]
        public string Descripcion { get; set; } = string.Empty;
    }
}
