namespace BlumeAPI.Repository;
using BlumeAPI.Data.Entities;
public interface IFacturaRepository
{
    //ORM
    Task<FacturaEntity?> GetByIdAsync(int idFactura);
    //DAPPER
}
