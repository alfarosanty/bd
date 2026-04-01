using BlumeAPI.Entities;

namespace BlumeAPI.Repository;
public interface IMedidaRepository
{
    Task<Medida?> GetById(int id);
}