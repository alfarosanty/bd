using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;

public class ARCARepository : IARCARepository
{
    private readonly AppDbContext _context;

    public ARCARepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DatosAutenticacion?> ObtenerAutenticacionAsync()
    {
        return await _context.DatosAutenticacion.FirstOrDefaultAsync();
    }

    public async Task<DatosAfip?> ObtenerConfiguracionAsync()
    {
        return await _context.DatosAfip.FirstOrDefaultAsync();
    }

    public async Task GuardarAutenticacionAsync(DatosAutenticacion auth)
    {
        var existente = await _context.DatosAutenticacion.FirstOrDefaultAsync();

        if (existente != null)
        {
            _context.DatosAutenticacion.Remove(existente);
            

            await _context.SaveChangesAsync();
        }

        _context.DatosAutenticacion.Add(auth);
        
        await _context.SaveChangesAsync();
    }
}