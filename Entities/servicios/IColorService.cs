using BlumeAPI;
using BlumeAPI.Models;

public interface IColorService
{
    Task<List<Color>> ListarColoresAsync();
}
