namespace BlumeAPI.Entities
{
public class PedidoProduccionIngresoDetalle
{
    public static string TABLA = "PEDIDO_PRODUCCION_INGRESO_DETALLE";

    public int Id { get; set; } // Clave primaria

    // Foreign Keys explícitas
    public int IdPedidoProduccion { get; set; }
    public PedidoProduccion PedidoProduccion { get; set; }

    public int IdIngreso { get; set; }
    public Ingreso Ingreso { get; set; }

    public int? IdPresupuesto { get; set; }
    public Presupuesto? Presupuesto { get; set; }

    public int IdArticulo { get; set; }
    public Articulo Articulo { get; set; }

    // Datos del movimiento de stock
    public int CantidadDescontada { get; set; }
    public int CantidadPendienteAntes { get; set; }
    public int CantidadPendienteDespues { get; set; }
}
    
}
