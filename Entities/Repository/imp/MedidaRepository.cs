using BlumeApi.Models;
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class MedidaRepository:IMedidaRepository{

private readonly AppDbContext _context;

    public MedidaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Medida>> GetMedidasAsync()
    {
        return await _context.Medidas.ToListAsync();
    }

}