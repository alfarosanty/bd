using BlumeApi.Models;
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class TallerRepository : ITallerRepository
{

    private readonly AppDbContext _context;

    public TallerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Taller>> listarTalleresAsync()
    {
        return await _context.Talleres.ToListAsync(); 
    }

}