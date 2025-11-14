using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeApi.Models;
using BlumeAPI.Models;

namespace BlumeApi.Models
{
    [Table("PEDIDO_PRODUCCION_INGRESO_DETALLE")]
    public class PedidoProduccionIngresoDetalle
    {
        public static string TABLA = "PEDIDO_PRODUCCION_INGRESO_DETALLE";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_PEDIDO_PRODUCCION_INGRESO_DETALLE")]
        public int Id { get; set; }

        [Column("ID_PEDIDO_PRODUCCION")]
        public int IdPedidoProduccion { get; set; }
        public PedidoProduccion PedidoProduccion { get; set; } = null!;

        [Column("ID_INGRESO")]
        public int IdIngreso { get; set; }
        public Ingreso Ingreso { get; set; } = null!;

        [Column("ID_PRESUPUESTO")]
        public int? IdPresupuesto { get; set; }
        public Presupuesto? Presupuesto { get; set; }

        [Column("ID_ARTICULO")]
        public int IdArticulo { get; set; }
        public Articulo Articulo { get; set; } = null!;

        [Column("CANTIDAD_DESCONTADA")]
        public int CantidadDescontada { get; set; }

        [Column("CANTIDAD_PENDIENTE_ANTES")]
        public int CantidadPendienteAntes { get; set; }

        [Column("CANTIDAD_PENDIENTE_DESPUES")]
        public int CantidadPendienteDespues { get; set; }
    }
}
