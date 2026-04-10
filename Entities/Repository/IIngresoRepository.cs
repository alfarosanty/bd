using BlumeAPI.Entities;

namespace BlumeAPI.Repository
{
    public interface IIngresoRepository
    {
        Task<Ingreso> Crear(Ingreso ingreso);
        Task Actualizar(Ingreso ingreso);
        Task CrearDetallesIngresoPedidoProduccion(List<PedidoProduccionIngresoDetalle> detalles);
        Task<PagedResult<Ingreso>> GetByTaller(int idTaller, DateTime desde, DateTime hasta, EstadoIngreso? estado, int page, int pageSize);
        Task<Ingreso> GetById(int id);
        Task<List<Ingreso>> GetByIds(List<int> ids);
        Task<List<PedidoProduccionIngresoDetalle>> GetDetallesPPI(int idIngreso);
        Task<bool> Existe(int id);
        Task Eliminar(Ingreso ingreso);
    }
}