
using BlumeAPI.Models;

public interface IArticuloService
{
    Task<Articulo?> GetArticuloAsync(int id);
    Task<List<Articulo>> GetAllAsync();
    Task<List<int>> CrearArticulosAsync(List<Articulo> articulos);
    Task <List<ConsultaMedida>> ConsultarMedidasNecesarias(ArticuloPresupuesto[] articulosPresupuesto);
    Task<List<ConsultaTallerCortePorCodigo>> ConsultarCantidadesTallerCorte(string codigo);
    Task<List<ConsultaTallerCortePorCodigo>> ConsultarTodosArticulosCantidadesTallerCorte();
    Task<int> ActualizarStockAsync(ActualizacionStockInutDTO[] articulos);

// ============================== ARTICULO PRECIO ==============================
    Task<List<ArticuloPrecio>> GetArticulosPrecioAsync();
    Task<List<int>> CrearArticulosPreciosAsync(ArticuloPrecio[] articuloPrecios);
    Task<List<int>> ActualizarArticulosPreciosAsync(ArticuloPrecio[] articuloPrecios);
    Task<Dictionary<int, ArticuloPrecio>> ObtenerPreciosPorIdsAsync(int[] ids, bool habilitados);
    Task<EstadisticaArticuloDTO> GetArticuloPresupuestadoAsync(int idArticuloPrecio, DateTime? fechaDesde, DateTime? fechaHasta);




}
