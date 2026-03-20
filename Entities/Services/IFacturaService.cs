namespace BlumeAPI.Services;
using BlumeAPI.Data.Entities;
using BlumeAPI.Entities.clases.modelo;

public interface IFacturaService
{
    //Task<int> crear(Factura factura, Npgsql.NpgsqlConnection connection);
    //Task<Factura> CrearConAFIPAsync(Factura factura);
    Task<FacturaEntity> GetByIdAsync(int idFactura);
    Task<NotaDeCredito> CrearNotaCreditoAsync(NotaDeCredito notaDeCredito);
    Task<AfipResponse?> ValidarNotaCreditoWsfeAsync(NotaDeCredito notaDeCredito, LoginTicketResponseData loginTicket, long cuitRepresentada);
    Task ActualizarNotaCreditoDatosAfipAsync(int idInterno, AfipResponse respuestaAfip);



}
