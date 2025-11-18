using BlumeAPI.Models;

public interface IArticuloPrecioRepository
{

    Task<List<ArticuloPrecio>> GetAllAsync();
    Task<List<int>> CrearArticulosPreciosAsync(ArticuloPrecio[] articuloPrecios);
    Task<List<int>> ActualizarArticulosPreciosAsync(ArticuloPrecio[] articuloPrecios);
    Task<Dictionary<int, ArticuloPrecio>> ObtenerPreciosPorIdsAsync(int[] ids, bool habilitados);
    Task<EstadisticaArticuloDTO> GetArticuloPresupuestadoAsync(int idArticuloPrecio, DateTime? fechaDesde, DateTime? fechaHasta);



}