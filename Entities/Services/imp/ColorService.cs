

using BlumeAPI.Entities;
using BlumeAPI.Services;
using Npgsql;

public class ColorService : IColorService
{
    private readonly IUnitOfWork _unitOfWork;

    public ColorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Color?> GetById(int id)
    {
        return await _unitOfWork.Colores.GetById(id);
    }

    public async Task<List<Color>> getAll()
    {
        return await _unitOfWork.Colores.GetAllAsync();
    }

}