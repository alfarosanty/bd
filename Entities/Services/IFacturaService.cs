namespace BlumeAPI.Services;
using BlumeAPI.Entities.clases.modelo;

public interface IFacturaService
{
    //Task<int> crear(Factura factura, Npgsql.NpgsqlConnection connection);
    //Task<Factura> CrearConAFIPAsync(Factura factura);
    Task<Factura> GetByIdAsync(int idFactura);
    Task<PagedResult<Factura>> GetFacturasAsync(DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize);
    Task<PagedResult<Factura>> GetFacturasByClienteAsync(int idCliente, DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize);
    Task<NotaDeCredito> CrearNotaCreditoAsync(NotaDeCredito notaDeCredito);
    Task<AfipResponse?> ValidarNotaCreditoWsfeAsync(NotaDeCredito notaDeCredito, LoginTicketResponseData loginTicket, long cuitRepresentada);
    Task ActualizarNotaCreditoDatosAfipAsync(int idInterno, AfipResponse respuestaAfip);



}
