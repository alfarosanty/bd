using BlumeAPI;

public interface IColorService
{
    Task<List<Color>> ListarColoresAsync();
}
