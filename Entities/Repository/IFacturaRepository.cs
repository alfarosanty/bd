namespace BlumeAPI.Repository;
using BlumeAPI.Data.Entities;
using BlumeAPI.Entities.clases.modelo;

public interface IFacturaRepository
{
    //ORM
    Task<FacturaEntity?> GetByIdAsync(int idFactura);
    //DAPPER

    // NOTA DE CRÉDITO
    Task<int> InsertarNotaCreditoAsync(NotaDeCredito notaDeCredito);
    Task InsertarArticulosNotaCreditoAsync(List<ArticuloNotaCredito> articulos);
    Task ActualizarNotaDeCreditoAfipAsync(int id, AfipResponse respuesta);
}
