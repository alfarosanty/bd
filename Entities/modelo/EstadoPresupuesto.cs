using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlumeApi.Models
{
    [Table("ESTADO_PRESUPUESTO")]
    public class EstadoPresupuesto
    {
        public static string TABLA = "ESTADO_PRESUPUESTO";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_ESTADO_PRESUPUESTO")]
        public int Id { get; set; }

        [Column("CODIGO")]
        public string? Codigo { get; set; }

        [Column("DESCRIPCION")]
        public string? Descripcion { get; set; }
    }
}
