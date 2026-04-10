using BlumeAPI.Entities;

namespace BlumeAPI.Repository;
public interface IColorRepository
{
    Task<List<Color>> GetAllAsync();
    Task<Color?> GetById(int id);
}
