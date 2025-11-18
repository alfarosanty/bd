using BlumeApi.Models;

namespace BlumeAPI.Repository{

public interface IPresupuestoRepository{

        Task<Presupuesto?>GetPresupuesto(int idPresupuesto);


}

}