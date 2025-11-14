using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeAPI.Models;

namespace BlumeApi.Models
{
    [Table("PRESUPUESTO")]
    public class Presupuesto
    {
        public static string TABLA = "PRESUPUESTO";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_PRESUPUESTO")]
        public int Id { get; set; }

        [Column("FECHA")]
        public DateTime? Fecha { get; set; }

        [Column("ID_CLIENTE")]
        public int? IdCliente { get; set; }
        public Cliente? Cliente { get; set; }

        [Column("EXIMIR_IVA")]
        public bool? EximirIVA { get; set; }

        [Column("ID_ESTADO_PRESUPUESTO")]
        public int? IdEstadoPresupuesto { get; set; }
        public EstadoPresupuesto? EstadoPresupuesto { get; set; }

        [Column("ID_FACTURA")]
        public int? IdFactura { get; set; }
        public Factura? Factura { get; set; }

        [InverseProperty("Presupuesto")]
        public List<ArticuloPresupuesto>? Articulos { get; set; }

        [Column("DESCUENTO_GENERAL", TypeName = "numeric(6,2)")]
        public float? DescuentoGeneral { get; set; }
    }
}
