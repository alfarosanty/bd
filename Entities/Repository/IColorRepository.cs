using BlumeAPI.Entities.clases.modelo;

namespace BlumeAPI.Repositories;
public interface IColorRepository
{
    Task<Color?> GetById(int id);
}
