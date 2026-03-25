namespace BlumeAPI.Services;
public interface IPresupuestoService
{
    Task<PagedResult<Presupuesto>> GetPresupuestosAsync(DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize);
    Task<PagedResult<Presupuesto>> GetPresupuestosByClienteAsync(int idCliente, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize);
    Task<Presupuesto?> GetPresupuestoByIdAsync(int id);
}