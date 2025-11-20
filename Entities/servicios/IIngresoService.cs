using BlumeApi.Models;

namespace Entities.servicios
{
    public interface IIngresoService
    {
        Task<int> actualizarAsync(Ingreso ingreso);
        Task<int> CrearAsync(Ingreso ingreso);
        Task<List<int>> CrearDetallesIngresoPedidoProduccionAsync(List<PedidoProduccionIngresoDetalle> detalles);
        Task<List<Ingreso>> GetIngresoByTallerAsync(int idTaller);
        Task<Ingreso> GetIngresoAsync(int idIngreso);
        Task<List<Ingreso>> GetIngresosByIdsAsync(List<int> idsIngresos);
        Task<List<PedidoProduccionIngresoDetalle>> GetDetallesPPIAsync(int idIngreso);
        Task<int> BorrarIngresoAsync(Ingreso ingreso);
    }

}