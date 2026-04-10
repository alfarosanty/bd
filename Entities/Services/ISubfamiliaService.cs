namespace BlumeAPI.Services;
using BlumeAPI.Entities;
public interface ISubfamiliaService
{
    Task<List<SubFamilia>> getAll();
}