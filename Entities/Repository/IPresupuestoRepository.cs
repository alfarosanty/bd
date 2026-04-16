namespace BlumeAPI.Repository;
public interface IPresupuestoRepository
{
    Task<PagedResult<Presupuesto>> GetAll(DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize);
    Task<PagedResult<Presupuesto>> GetByCliente(int idCliente, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize);
    Task<Presupuesto?> GetById(int id);
    Task<Dictionary<int, string>> GetNombresClientesPorPresupuestos(List<int> idsPresupuesto);
    Task<List<EstadoPresupuesto>> GetEstados();
    void Crear(Presupuesto presupuesto);
    Task Actualizar(Presupuesto presupuesto);
}