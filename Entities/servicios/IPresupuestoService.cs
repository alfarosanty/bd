using BlumeApi.Models;

namespace BlumeAPI.Services{

public interface IPresupuestoService{

    Task<Presupuesto?>GetPresupuesto(int idPresupuesto);

}


}