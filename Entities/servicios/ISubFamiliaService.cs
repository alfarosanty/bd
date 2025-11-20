using BlumeApi.Models;

namespace BlumeAPI.Services{
    public interface ISubFamiliaService
    {
        Task<List<SubFamilia>> listarSubFamiliasAsync();
    }
}