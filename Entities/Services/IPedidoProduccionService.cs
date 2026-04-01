namespace BlumeAPI.Services;

public interface IPedidoProduccionService
{
    Task<int> CrearPedido(PedidoProduccion pedido);
    Task<PagedResult<PedidoProduccionDTO>> ListarPorTallerPaginado(int idTaller, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize);
    Task<List<int>> EliminarPedidos(List<int> ids);
    Task<List<EstadoPedidoProduccion>> ListarEstados();
    Task<PedidoProduccion> GetById(int id);
    Task<List<PedidoProduccion>> GetByIds(List<int> ids);
    Task Actualizar(PedidoProduccion pedido);
    Task ActualizarEstadoVarios(ActualizacionEstadosDTO dto);

}