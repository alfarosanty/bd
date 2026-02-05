using BlumeAPI.Data.Entities;

public interface IArticuloRepository
{
    // ORM
    Task<ArticuloPrecioEntity?> GetArticuloPrecioByIdAsync(int idArticuloPrecio);
    Task<ArticuloEntity?> GetByIdAsync(int idArticulo);
    // DAPPER
    Task<List<ArticuloFacturaEntity>> GetFacturadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta);
    Task<List<ArticuloIngresoEntity>> GetIngresadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta);
}

