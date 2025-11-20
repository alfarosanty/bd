
using BlumeApi.Models;

namespace BlumeAPI.Repository{
    public interface IFacturaRepository
    {
        Task<int> crear(Factura factura);
        Task CrearArticuloFacturaAsync(ArticuloFactura articuloFactura);
        Task<List<RespuestaEstadistica>> facturacionXCliente(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Factura>> getFacturasPorFiltro(int? idCliente, string? tipoFactura, int? puntoDeVenta, DateTime fechaInicio, DateTime fechaFin);
    }
}