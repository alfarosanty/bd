using BlumeAPI.Entities;

public interface IARCARepository
{
    Task<DatosAutenticacion?> ObtenerAutenticacionAsync();
    Task<DatosAfip?> ObtenerConfiguracionAsync();
    Task GuardarAutenticacionAsync(DatosAutenticacion auth);
}