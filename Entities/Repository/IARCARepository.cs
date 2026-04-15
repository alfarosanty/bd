using BlumeAPI.Entities;

public interface IARCARepository
{
    Task<DatosAutenticacion?> ObtenerAutenticacionPorServicioAsync(string servicio);
    Task<DatosAfip?> ObtenerConfiguracionAsync();
    Task GuardarAutenticacionAsync(DatosAutenticacion auth);
    Task<DatosAfip?> GetPrimerRegistroAsync();
    void Update(DatosAfip entidad);
}