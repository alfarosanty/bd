using BlumeAPI.Entities.clases.modelo;

namespace BlumeAPI.Repositories;
public interface IMedidaRepository
{
    Task<Medida?> GetById(int id);
}