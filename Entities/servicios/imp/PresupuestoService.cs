using BlumeApi.Models;
using BlumeAPI.Repository;
using BlumeAPI.Services;

public class PresupuestoService : IPresupuestoService{

    private readonly IPresupuestoRepository iPresupuestoReporitory;

    public PresupuestoService(IPresupuestoRepository _iPresupuestoRepository){
        iPresupuestoReporitory = _iPresupuestoRepository;
    }

    public Task<Presupuesto?> GetPresupuesto(int idPresupuesto)
    {
        return iPresupuestoReporitory.GetPresupuesto(idPresupuesto);
    }

    public async Task<int> CrearPresupuestoAsync(Presupuesto presupuesto)
    {
        return await iPresupuestoReporitory.CrearPresupuestoAsync(presupuesto);
    }
}