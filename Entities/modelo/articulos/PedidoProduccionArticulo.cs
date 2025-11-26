using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeAPI.Models;

namespace BlumeApi.Models
{
    [Table("PRODUCCION_ARTICULO")]
    public class PedidoProduccionArticulo
    {
        public static string TABLA = "PRODUCCION_ARTICULO";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_PRODUCCION_ARTICULO")]
        public int Id { get; set; }

        [ForeignKey("IdArticulo")]
        public Articulo Articulo { get; set; }

        [Column("ID_PEDIDO_PRODUCCION")]
        public int? IdPedidoProduccion { get; set; }

        [ForeignKey(nameof(IdPedidoProduccion))]
        public PedidoProduccion? PedidoProduccion { get; set; }

        [Column("CANTIDAD")]
        public int Cantidad { get; set; }

        [Column("CANTIDAD_PENDIENTE")]
        public int? CantidadPendiente { get; set; }

        [Column("HAY_STOCK")]
        public bool HayStock { get; set; }

        [Column("DESCRIPCION")]
        public string? Descripcion { get; set; }

        [Column("CODIGO")]
        public string Codigo { get; set; } = string.Empty;
    }
}
