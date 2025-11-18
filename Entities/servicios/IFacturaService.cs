using BlumeApi.Models;

namespace BlumeAPI.Services{

    public interface IFacturaService
    {
        Task<int> crear(Factura factura);
        Task<List<RespuestaEstadistica>> facturacionXCliente(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Factura>> getFacturasPorFiltro(int? idCliente, string? tipoFactura, int? puntoDeVenta, DateTime fechaInicio, DateTime fechaFin);
    }


}