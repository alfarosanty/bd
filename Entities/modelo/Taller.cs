using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlumeApi.Models
{
    [Table("FABRICANTE")]
    public class Taller
    {
        public static string TABLA = "FABRICANTE";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_FABRICANTE")]
        public int Id { get; set; }

        [Column("TELEFONO")]
        public string? Telefono { get; set; }

        [Column("RAZON_SOCIAL")]
        public string RazonSocial { get; set; } = string.Empty;

        [Column("DIRECCION")]
        public string? Direccion { get; set; }

        [Column("PROVINCIA")]
        public string? Provincia { get; set; }
    }
}
