using System.Data;
using BlumeAPI.Entities;
using BlumeAPI.Repository;
using Dapper;
using Microsoft.EntityFrameworkCore;

public class MedidaRepository : IMedidaRepository
{
private readonly IDbConnectionFactory _factory;
private readonly AppDbContext _context;

    public MedidaRepository(AppDbContext context, IDbConnectionFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public async Task<List<Medida>> GetAllAsync()
    {
        return await _context.Set<Medida>().AsNoTracking().ToListAsync();
    }

    public async Task<Medida?> GetByIdAsync(int id)
    {
        return await _context.Set<Medida>().FindAsync(id);
    }
}
