

using BlumeApi.Models;

namespace Entities.Repository
{
    public interface IIngresoRepository
    {
        Task<int> actualizarAsync(Ingreso ingreso);
        Task<int> BorrarIngresoAsync(Ingreso ingreso);
        Task<int> CrearAsync(Ingreso ingreso);
        Task<List<int>> CrearDetallesIngresoPedidoProduccionAsync(List<PedidoProduccionIngresoDetalle> detalles);
        Task CrearIngresoArticuloAsync(ArticuloIngreso ingresoArticulo);
        Task<List<PedidoProduccionIngresoDetalle>> GetDetallesPPIAsync(int idIngreso);
        Task<Ingreso> GetIngresoAsync(int idIngreso);
        Task<List<Ingreso>> GetIngresoByTallerAsync(int idTaller);
    }
}