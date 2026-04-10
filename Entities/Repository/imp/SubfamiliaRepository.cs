using System.Data;
using BlumeAPI.Entities;
using BlumeAPI.Repository;
using Dapper;
using Microsoft.EntityFrameworkCore;

public class SubfamiliaRepository : ISubfamiliaRepository
{
    private readonly IDbConnectionFactory _factory;
    private readonly AppDbContext _context;

    public SubfamiliaRepository(AppDbContext context, IDbConnectionFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public async Task<List<SubFamilia>> GetAllAsync()
        {
            return await _context.Set<SubFamilia>().AsNoTracking().ToListAsync();
        }

    public async Task<SubFamilia?> GetByIdAsync(int id)
    {
            return await _context.Set<SubFamilia>().FindAsync(id);
    }

}
