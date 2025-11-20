using BlumeApi.Models;

namespace BlumeAPI.Repository
{
    public interface IMedidaRepository
    {
        Task<List<Medida>> GetMedidasAsync();
    }
}