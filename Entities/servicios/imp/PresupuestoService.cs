using BlumeApi.Models;
using BlumeAPI.Models;
using BlumeAPI.Repository;
using BlumeAPI.Services;

public class PresupuestoService : IPresupuestoService{

    private readonly IPresupuestoRepository iPresupuestoRepository;

    public PresupuestoService(IPresupuestoRepository _iPresupuestoRepository){
        iPresupuestoRepository = _iPresupuestoRepository;
    }

    public Task<Presupuesto?> GetPresupuesto(int idPresupuesto)
    {
        return iPresupuestoRepository.GetPresupuesto(idPresupuesto);
    }

    public async Task<int> CrearPresupuestoAsync(Presupuesto presupuesto)
    {
        return await iPresupuestoRepository.CrearPresupuestoAsync(presupuesto);
    }

    public async Task<bool> ActualizarPresupuestoAsync(Presupuesto presupuesto)
{
    var existente = await iPresupuestoRepository.GetPresupuesto(presupuesto.Id);

    if (existente == null)
        return false;

    // cabecera
    existente.Fecha = presupuesto.Fecha;
    existente.IdCliente = presupuesto.IdCliente;
    existente.DescuentoGeneral = presupuesto.DescuentoGeneral;
    existente.IdEstadoPresupuesto = presupuesto.IdEstadoPresupuesto;

    var idsNuevos = presupuesto.Articulos?.Select(a => a.Id).ToList() ?? new List<int>();

    existente.Articulos.RemoveAll(a => !idsNuevos.Contains(a.Id));

    foreach (var art in presupuesto.Articulos)
    {
        var existingArt = existente.Articulos.FirstOrDefault(a => a.Id == art.Id);

        if (existingArt != null)
        {
            existingArt.Cantidad = art.Cantidad;
            existingArt.CantidadPendiente = art.CantidadPendiente;
            existingArt.PrecioUnitario = art.PrecioUnitario;
            existingArt.Descuento = art.Descuento;
            existingArt.Descripcion = art.Descripcion;
            existingArt.HayStock = art.HayStock;
            existingArt.IdArticulo = art.IdArticulo;
            existingArt.Codigo = art.Codigo;
        }
        else
        {
            existente.Articulos.Add(new ArticuloPresupuesto
            {
                IdArticulo = art.IdArticulo,
                Cantidad = art.Cantidad,
                CantidadPendiente = art.CantidadPendiente,
                PrecioUnitario = art.PrecioUnitario,
                Descuento = art.Descuento,
                Descripcion = art.Descripcion,
                HayStock = art.HayStock,
                Codigo = art.Codigo,
                IdPresupuesto = existente.Id
            });
        }
    }

    return await iPresupuestoRepository.ActualizarPresupuestoAsync(existente);
}

    public async Task<List<Presupuesto>>GetPresupuestoByCliente(int idCliente){

    return await iPresupuestoRepository.GetPresupuestoByCliente(idCliente);

    }

    public async Task<List<EstadoPresupuesto>> getEstadosPresupuesto(){

        return await iPresupuestoRepository.getEstadosPresupuesto();
    }

    public async Task<List<ArticuloPresupuesto>> articulosPresupuestados(int idArticuloPrecio, DateTime fechaInicio, DateTime fechaFin){

        return await iPresupuestoRepository.articulosPresupuestados(idArticuloPrecio, fechaInicio, fechaFin);

    }

    public async Task<List<Presupuesto>> GetPresupuestosByIds(List<int> idsPresupuestos){
        List<Presupuesto> presupuestos = new List<Presupuesto>();

        foreach (var id in idsPresupuestos)
        {
            var presupuesto = await iPresupuestoRepository.GetPresupuesto(id);
            if (presupuesto != null)
            {
                presupuestos.Add(presupuesto);
            }
        }

        return presupuestos;
    }



}