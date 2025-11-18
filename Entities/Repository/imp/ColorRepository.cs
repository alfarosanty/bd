using BlumeAPI;
using BlumeAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ColorRepository : IColorRepository
{
    private readonly AppDbContext _context;

    public ColorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Color>> ListarColoresAsync()
    {
        return await _context.Colores.ToListAsync();
    }
}

