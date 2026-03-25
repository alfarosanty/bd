using BlumeAPI.Data.Entities;
using BlumeAPI.Entities.clases.modelo;
using Npgsql;

public interface IArticuloRepository
{
    // ORM
    Task<ArticuloPrecio?> GetArticuloPrecioByIdAsync(int idArticuloPrecio);
    Task<Articulo?> GetByIdAsync(int idArticulo);
    //Task<List<ArticuloEntity>> GetArticulosByArticuloPrecioIdAsync(int articuloPrecioId, bool habilitados);
    // DAPPER
    Task<List<CartaKardexDTO>> GetFacturadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta);
    Task<List<CartaKardexDTO>> GetIngresadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta);

    Task RestaurarStockAsync(List<IArticuloConStock> articulos, NpgsqlConnection conn, NpgsqlTransaction tran);
    }

