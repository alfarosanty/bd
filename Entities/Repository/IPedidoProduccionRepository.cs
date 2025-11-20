using BlumeApi.Models;

namespace BlumeAPI.Repository{
    public interface IPedidoProduccionRepository
    {
        Task<int> ActualizarAsync(PedidoProduccion pedidoProduccion);
        Task CrearArticuloAsync(PedidoProduccionArticulo articuloProduccion);
        Task<int?> CrearAsync(PedidoProduccion pedidoProduccion);
        Task<int> EliminarPedidoProduccionAsync(int idPedidoProduccio);
        Task<List<EstadoPedidoProduccion>> GetEstadosPedidoProduccionAsync();
        Task<PedidoProduccion> GetPedidoProduccionAsync(int id);
        Task<List<PedidoProduccion>> GetPedidoProduccionByTallerAsync(int idTaller);
    }
}