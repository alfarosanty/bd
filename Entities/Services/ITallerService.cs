namespace BlumeAPI.Services
{
public interface ITallerService
{
    Task<List<Taller>> GetAll();
}
}