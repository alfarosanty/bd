using BlumeApi.Models;

namespace BlumeAPI.Services
{
    public interface ITallerService
    {
        Task<List<Taller>> listarTalleresAsync();
    }
}