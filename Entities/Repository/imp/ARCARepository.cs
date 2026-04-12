using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;

public class ARCARepository : IARCARepository
{
    private readonly AppDbContext _context;

    public ARCARepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DatosAutenticacion?> ObtenerAutenticacionPorServicioAsync(string servicio)
    {
        return await _context.DatosAutenticacion
            .FirstOrDefaultAsync(a => a.Servicio == servicio);
    }

    public async Task<DatosAfip?> ObtenerConfiguracionAsync()
    {
        return await _context.DatosAfip.FirstOrDefaultAsync();
    }

    public async Task GuardarAutenticacionAsync(DatosAutenticacion auth)
    {
        var existente = await _context.DatosAutenticacion
                                    .FirstOrDefaultAsync(a => a.Servicio == auth.Servicio);

        if (existente != null)
        {
            existente.Token = auth.Token;
            existente.Firma = auth.Firma;
            existente.Expiracion = auth.Expiracion;
            existente.UniqueId = auth.UniqueId;
            // _context.Update(existente); // Opcional, EF detecta cambios automáticamente
        }
        else
        {
            _context.DatosAutenticacion.Add(auth);
        }

        await _context.SaveChangesAsync();
    }
}