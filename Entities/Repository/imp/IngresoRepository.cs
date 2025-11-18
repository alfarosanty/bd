using Entities.Repository;

public class IngresoRepository : IIngresoRepository
{

private readonly AppDbContext _context;
    public IngresoRepository(AppDbContext context)
    {
        _context = context;

    }
}