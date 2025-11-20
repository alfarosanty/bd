using BlumeApi.Models;

namespace BlumeAPI.servicios
{
    public interface IPedidoProduccionService
    {
        Task<int> ActualizarAsync(PedidoProduccion pedidoProduccion);
        Task<List<int>> actualizarEstadosPedidoProduccionAsync(List<PedidoProduccionEstadoDTO> lista);
        Task<int> CrearAsync(PedidoProduccion pedidoProduccion);
        Task <List<int>>eliminarPedidosProduccion(List<int> idPedidos);
        Task<List<EstadoPedidoProduccion>> GetEstadosPedidoProduccionAsync();
        Task<PedidoProduccion> GetPedidoProduccionAsync(int idPedidoProduccion);
        Task<List<PedidoProduccion>> GetPedidoProduccionByTaller(int idTaller);
        Task<List<PedidoProduccion>> GetPedidosProduccionByIdsAsync(List<int> idsPedidosProduccion);
        Task<List<ClienteXPedidoProduccionOutputDTO>> obtenerClientesAsync(List<int> idPedidos);
    }
}