namespace BlumeAPI.Repository
{
    public interface IPedidoProduccionRepository
    {
        Task<int> Crear(PedidoProduccion pedido);
        Task<PagedResult<PedidoProduccion>> GetByTallerPaginado(int idTaller, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize);    
        Task<List<int>> EliminarVarios(List<int> ids);
        Task Actualizar(PedidoProduccion pedido);
        Task<List<EstadoPedidoProduccion>> GetEstados();
        Task<PedidoProduccion> GetById(int id);
        Task<List<PedidoProduccion>> GetByIds(List<int> ids);
        Task<bool> Existe(int id);
        Task<int> ActualizarEstadosMasivos(List<int> ids, EstadoPedidoProduccion nuevoEstadoId);

    }
}