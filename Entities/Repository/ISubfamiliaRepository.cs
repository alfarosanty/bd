using BlumeAPI.Entities.clases.modelo;

namespace BlumeAPI.Repositories;
public interface ISubfamiliaRepository
{
    Task<SubFamilia?> GetById(int id);
}