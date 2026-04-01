using BlumeAPI.Entities.Repository;
using Microsoft.EntityFrameworkCore;

public class TallerRepository : ITallerRepository
{
    private readonly AppDbContext _context;

    public TallerRepository(AppDbContext contexto)
    {
        _context = contexto;
    }

    public async Task<List<Taller>> GetAll()
    {
        return await _context.Talleres
        .AsNoTracking()
        .ToListAsync();
    }
}