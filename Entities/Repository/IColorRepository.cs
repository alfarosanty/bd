using BlumeAPI;
using BlumeAPI.Models;

public interface IColorRepository
{
    Task<List<Color>> ListarColoresAsync();
}
