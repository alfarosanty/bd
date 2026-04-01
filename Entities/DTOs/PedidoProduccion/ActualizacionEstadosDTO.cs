public class ActualizacionEstadosDTO
{
    public List<int> PedidoIds { get; set; }
    public EstadoPedidoProduccion NuevoEstado { get; set; }
}