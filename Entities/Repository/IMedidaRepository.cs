using BlumeAPI.Entities;

namespace BlumeAPI.Repository;
public interface IMedidaRepository
{
    Task<List<Medida>> GetAllAsync();
    Task<Medida?> GetByIdAsync(int id);
}