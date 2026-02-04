namespace BlumeAPI.Services;

public interface IArticuloService
{
    Task<Articulo?> GetArticulo(int idArticulo);
    Task<List<ArticuloFactura>> GetFacturadosByArticulo(int idArticulo);
    Task<List<ArticuloIngreso>> GetIngresadosByArticulo(int idArticulo);
}
