using System.Data;
using BlumeAPI.Entities;
using BlumeAPI.Repository;
using Dapper;
using Microsoft.EntityFrameworkCore;

public class ColorRepository : IColorRepository
{
private readonly IDbConnectionFactory _factory;
private readonly AppDbContext _context;

    public ColorRepository(AppDbContext context, IDbConnectionFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public async Task<Color?> GetById(int idColor)
        {
            using var conn = _factory.CreateConnection();

            var sql = @"
                SELECT
                    ""ID_COLOR"" AS ""Id"",
                    ""CODIGO"",
                    ""DESCRIPCION"",
                    ""HEXA"" AS ""ColorHexa""
                FROM ""COLOR""
                WHERE ""ID_COLOR"" = @IdColor;
            ";

            return await conn.QueryFirstOrDefaultAsync<Color>(sql, new { IdColor = idColor });
        }

        public async Task<List<Color>> GetAllAsync()
        {
            return await _context.Set<Color>().AsNoTracking().ToListAsync();
        }
}
