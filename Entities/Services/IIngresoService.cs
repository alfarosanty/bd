using BlumeAPI.Entities;

namespace BlumeAPI.Services
{
public interface IIngresoService
    {
        Task<Ingreso> CrearIngresoConDescuentoAsync(Ingreso ingreso);
        Task ActualizarIngreso(Ingreso ingreso);
        Task<PagedResult<Ingreso>> GetIngresosByTaller(int idTaller, DateTime desde, DateTime hasta, int page, int pageSize);
        Task<Ingreso> GetById(int id);
        Task<List<Ingreso>> GetByIds (List<int> ids);
        Task<List<PedidoProduccionIngresoDetalle>> GetDetallesPPI (int idIngreso);
        Task<List<int>> EliminarIngresos(List<int> ids);
    }

}