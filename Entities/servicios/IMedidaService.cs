using BlumeApi.Models;

namespace BlumeAPI.Services{
    public interface IMedidaService
    {
        Task<List<Medida>> GetMedidasAsync();
    }
}