using BlumeApi.Models;
using BlumeAPI.Models;

namespace BlumeAPI.Services{

public interface IPresupuestoService{

    Task<Presupuesto?>GetPresupuestoAsync(int idPresupuesto);
    Task<int>CrearPresupuestoAsync(Presupuesto presupuesto);
    Task<bool>ActualizarPresupuestoAsync(Presupuesto presupuesto);
    Task<List<Presupuesto>>GetPresupuestoByCliente(int idCliente);
    Task<List<EstadoPresupuesto>> getEstadosPresupuesto();
    Task<List<ArticuloPresupuesto>> articulosPresupuestados(int idArticuloPrecio, DateTime fechaInicio, DateTime fechaFin);
    Task<List<Presupuesto>> GetPresupuestosByIds(List<int> idsPresupuestos);
    }


}