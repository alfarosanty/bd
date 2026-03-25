namespace BlumeAPI.Repositories;
public interface IPresupuestoRepository
{
    Task<PagedResult<Presupuesto>> GetAll(DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize);
    Task<PagedResult<Presupuesto>> GetByCliente(int idCliente, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize);
    Task<Presupuesto?> GetById(int id);
}