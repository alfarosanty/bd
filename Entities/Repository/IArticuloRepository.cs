using BlumeAPI.Models;

public interface IArticuloRepository
{
    Task<List<Articulo>> GetAllAsync();
    Task<Articulo> GetByIdAsync(int id);
    Task<List<int>> CrearArticulosAsync(List<Articulo> articulos);
    Task <List<ConsultaMedida>> ConsultarMedidasNecesarias(ArticuloPresupuesto[] articulosPresupuesto);
    Task< List<ConsultaTallerCorte>> ConsultarStockAsync(string codigo);
    Task< List<ConsultaTallerCorte>> ConsultarEnCorteAsync(string codigo);
    Task< List<ConsultaTallerCorte>> ConsultarEnTallerAsync(string codigo);
    Task< List<ConsultaTallerCorte>> ConsultarSeparadosAsync(string codigo);
    Task<int> ActualizarStockAsync(ActualizacionStockInutDTO[] articulos);
    Task<List<Articulo>> GetArticulosByArticuloPrecioIdAsync(int articuloPrecioId, bool habilitados);
    
}