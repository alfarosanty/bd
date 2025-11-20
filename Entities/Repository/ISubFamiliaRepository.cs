namespace BlumeAPI.Repository{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BlumeApi.Models;
    public interface ISubFamiliaRepository
    {
        Task<List<SubFamilia>> listarSubFamiliasAsync();
    }
}