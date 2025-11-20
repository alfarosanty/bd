using BlumeApi.Models;
using BlumeAPI.Models;

namespace BlumeAPI.Repository{

public interface IPresupuestoRepository{

        Task<Presupuesto?>GetPresupuesto(int idPresupuesto);
        Task<int>CrearPresupuestoAsync(Presupuesto presupuesto);
        Task<bool>ActualizarPresupuestoAsync(Presupuesto presupuesto);
        Task<List<Presupuesto>> GetPresupuestoByCliente(int idCliente);
        Task<List<EstadoPresupuesto>> getEstadosPresupuesto();
        Task<List<ArticuloPresupuesto>> articulosPresupuestados(int idArticuloPrecio, DateTime fechaInicio, DateTime fechaFin);
        Task AgregarArticuloPresupuestoAsync(ArticuloPresupuesto articulo);
    }

}