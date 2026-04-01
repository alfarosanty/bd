namespace BlumeAPI.Repository;
using BlumeAPI.Entities;

public interface IFacturaRepository
{
    //ORM
    Task<PagedResult<Factura>> GetAll(DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize);
    Task<PagedResult<Factura>> GetByCliente(int idCliente, DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize);
    
    Task<Factura?> GetByIdAsync(int idFactura);
    //DAPPER

    // NOTA DE CRÉDITO
    Task<int> InsertarNotaCreditoAsync(NotaDeCredito notaDeCredito);
    Task InsertarArticulosNotaCreditoAsync(List<ArticuloNotaCredito> articulos);
    Task ActualizarNotaDeCreditoAfipAsync(int id, AfipResponse respuesta);
}
