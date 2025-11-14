using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeApi.Models;

namespace BlumeApi.Models
{
    [Table("PEDIDO_PRODUCCION")]
    public class PedidoProduccion
    {
        public static string TABLA = "PEDIDO_PRODUCCION";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_PEDIDO_PRODUCCION")]
        public int Id { get; set; }

        [Column("FECHA")]
        public DateTime Fecha { get; set; }

        [Column("ID_TALLER")]
        public int IdTaller { get; set; }
        public Taller Taller { get; set; } = null!;

        [Column("ID_ESTADO_PEDIDO_PRODUCCION")]
        public int IdEstadoPedidoProduccion { get; set; }
        public EstadoPedidoProduccion EstadoPedidoProduccion { get; set; } = null!;

        [Column("ID_PRESUPUESTO")]
        public int? IdPresupuesto { get; set; }
        public Presupuesto? Presupuesto { get; set; }

        [InverseProperty("PedidoProduccion")]
        public List<PedidoProduccionArticulo> Articulos { get; set; } = new();
    }
}
