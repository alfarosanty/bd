using BlumeApi.Models;

namespace BlumeAPI.Repository{

public interface IPresupuestoRepository{

        Task<Presupuesto?>GetPresupuesto(int idPresupuesto);
        Task<int?>CrearPresupuestoAsync(Presupuesto presupuesto);



}

}