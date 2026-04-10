using BlumeAPI.Entities;

namespace BlumeAPI.Repository;
public interface ISubfamiliaRepository
{
    Task<SubFamilia?> GetByIdAsync(int id);
    Task<List<SubFamilia>> GetAllAsync();
}