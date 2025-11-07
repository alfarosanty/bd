using BlumeAPI;

public interface IColorRepository
{
    Task<List<Color>> ListarColoresAsync();
}
