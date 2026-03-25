using BlumeAPI.Repositories;
using BlumeAPI.Services;

public class PresupuestoServiceNUEVO : IPresupuestoService
{
    private readonly IPresupuestoRepository _presupuestoRepository;

    public PresupuestoServiceNUEVO(IPresupuestoRepository presupuestoRepository)
    {
        _presupuestoRepository = presupuestoRepository;
    }

// Service
public async Task<PagedResult<Presupuesto>> GetPresupuestosAsync(
    DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize)
{
    // Ya no retorna List<>, retorna PagedResult<>
    return await _presupuestoRepository.GetAll(desde, hasta, idEstado, page, pageSize);
}

public async Task<PagedResult<Presupuesto>> GetPresupuestosByClienteAsync(
    int idCliente, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize)
{
    return await _presupuestoRepository.GetByCliente(idCliente, desde, hasta, idEstado, page, pageSize);
}

    public async Task<Presupuesto?> GetPresupuestoByIdAsync(int id)
    {
        return await _presupuestoRepository.GetById(id);
    }
}