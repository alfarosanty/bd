using BlumeAPI.Entities.Repository;
using BlumeAPI.Services;

public class TallerService : ITallerService
{
    private readonly ITallerRepository _tallerRepository;

    public TallerService(ITallerRepository tallerRepository)
    {
        _tallerRepository = tallerRepository;
    }

    public async Task<List<Taller>> GetAll()
    {
        return await _tallerRepository.GetAll();
    }
}