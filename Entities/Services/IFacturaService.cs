namespace BlumeAPI.Services;
using BlumeAPI.Entities;

public interface IFacturaService
{
    Task<int> Crear(Factura factura, bool facturarEnArca);
    Task<Factura> GetByIdAsync(int idFactura);
    Task<PagedResult<Factura>> GetFacturasAsync(DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize);
    Task<PagedResult<Factura>> GetFacturasByClienteAsync(int idCliente, DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize);
    Task<NotaDeCredito> CrearNotaCreditoAsync(NotaDeCredito notaDeCredito);
    Task<AfipResponse?> ValidarNotaCreditoWsfeAsync(NotaDeCredito notaDeCredito, LoginTicketResponseData loginTicket, long cuitRepresentada);
    Task ActualizarNotaCreditoDatosAfipAsync(int idInterno, AfipResponse respuestaAfip);
    Task<AfipResponse?> FacturarWsfeAsync(Factura factura, LoginTicketResponseData loginTicket, long cuitRepresentada);
    Task ActualizarDatosAFIPAsync(int idFactura, AfipResponse respuesta);
    Dictionary<string, List<ArticuloFactura>> AgruparPorCodigo(List<ArticuloFactura> articulos);
    List<ArticuloResumen> ConstruirResumen(Dictionary<string, List<ArticuloFactura>> mapa);
}
