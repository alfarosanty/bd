using BlumeAPI.Entities;
using BlumeAPI.Services;

public class SubfamiliaService : ISubfamiliaService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubfamiliaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SubFamilia>> getAll()
        {
            var lista = await _unitOfWork.Subfamilias.GetAllAsync();
            return lista.ToList();
        }
}