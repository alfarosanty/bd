using BlumeApi.Models;
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class SubFamiliaRepository:ISubFamiliaRepository{

    private readonly AppDbContext _context;

    public SubFamiliaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubFamilia>> listarSubFamiliasAsync()
    {
        return await _context.SubFamilias.ToListAsync();
    }
}