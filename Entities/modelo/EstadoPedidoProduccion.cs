using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlumeApi.Models
{
    [Table("ESTADO_PEDIDO_PRODUCCION")]
    public class EstadoPedidoProduccion
    {
        public static string TABLA = "ESTADO_PEDIDO_PRODUCCION";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_ESTADO_PEDIDO_PRODUCCION")]
        public int Id { get; set; }

        [Column("CODIGO")]
        public string? Codigo { get; set; }

        [Column("DESCRIPCION")]
        public string? Descripcion { get; set; }
    }
}
