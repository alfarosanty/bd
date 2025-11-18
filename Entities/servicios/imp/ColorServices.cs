using BlumeAPI;
using BlumeAPI.Models;

public class ColorService : IColorService
{
    private readonly IColorRepository _colorRepository;

    public ColorService(IColorRepository colorRepository)
    {
        _colorRepository = colorRepository;
    }

    public async Task<List<Color>> ListarColoresAsync()
    {
        return await _colorRepository.ListarColoresAsync();
    }
}
