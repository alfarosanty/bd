using BlumeApi.Models;

namespace BlumeAPI.Repository
{
    public interface ITallerRepository
    {
        Task<List<Taller>> listarTalleresAsync();
    }
}