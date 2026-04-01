namespace BlumeAPI.Entities.Repository
{
public interface ITallerRepository
{
    Task<List<Taller>> GetAll();
}
}