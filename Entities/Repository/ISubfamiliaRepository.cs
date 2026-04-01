using BlumeAPI.Entities;

namespace BlumeAPI.Repository;
public interface ISubfamiliaRepository
{
    Task<SubFamilia?> GetById(int id);
}