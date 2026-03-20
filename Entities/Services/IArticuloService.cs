namespace BlumeAPI.Services;

public interface IArticuloService
{
    Task<Articulo?> GetArticulo(int idArticulo);
    Task<List<CartaKardexDTO>> GetFacturadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta);
    Task<List<CartaKardexDTO>> GetIngresadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta);
    Task<List<CartaKardexDTO>> GetResumenKardex(int idArticulo, DateTime? desde, DateTime? hasta);
}
