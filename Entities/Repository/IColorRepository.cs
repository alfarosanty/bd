using BlumeAPI.Entities;

namespace BlumeAPI.Repository;
public interface IColorRepository
{
    Task<Color?> GetById(int id);
}
