namespace BlumeAPI.Services;
using BlumeAPI.Data.Entities;

public interface IFacturaService
{
    //Task<int> crear(Factura factura, Npgsql.NpgsqlConnection connection);
    //Task<Factura> CrearConAFIPAsync(Factura factura);
    Task<FacturaEntity> GetByIdAsync(int idFactura);


}
