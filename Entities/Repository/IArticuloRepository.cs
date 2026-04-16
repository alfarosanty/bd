using BlumeAPI.Entities;
using Npgsql;

public interface IArticuloRepository
{
    // ORM
    Task<ArticuloPrecio?> GetArticuloPrecioByIdAsync(int idArticuloPrecio);
    Task<Articulo?> GetByIdAsync(int idArticulo);
    Task Update(Articulo articulo);
    Task<List<Articulo>> GetAllAsync(bool? habilitados);
    Task<List<ArticuloPrecio>> GetAllArticulosPrecioAsync(bool? habilitados);
    // DAPPER
    Task<List<CartaKardexDTO>> GetFacturadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta);
    Task<List<CartaKardexDTO>> GetIngresadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta);
    Task<List<Articulo>> GetByIdsAsync(List<int> ids);
    Task DescontarStockAsync(List<ArticuloFactura> articulos);

    Task RestaurarStockAsync(List<IArticuloConStock> articulos, NpgsqlConnection conn, NpgsqlTransaction tran);
    }

