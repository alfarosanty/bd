

using BlumeAPI.Entities;
using BlumeAPI.Services;
using Npgsql;

public class MedidaServices: IMedidaService 
{
    private readonly IUnitOfWork _unitOfWork;

    public MedidaServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Medida>> getAll()
    {
        return await _unitOfWork.Medidas.GetAllAsync();
    }

}