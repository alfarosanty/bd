using BlumeAPI.Data.Entities;
using BlumeAPI.Services;
using BlumeAPI.Repository;
  
class FacturaServicesNUEVO : IFacturaService
{
private IFacturaRepository _facturaRepository;

public FacturaServicesNUEVO(IFacturaRepository facturaRepository)
{
    _facturaRepository = facturaRepository;
}

public Task<FacturaEntity> GetByIdAsync(int idFactura)
{
    var facturaResponse = _facturaRepository.GetByIdAsync(idFactura);
    return facturaResponse;
}



// FUNCIONES AUXILIARES 
/*private void calcularMontos(int descuentoGeneral, ArticuloFactura articuloFactura)
    {
        var precioUnitario = articuloFactura.precioUnitario;
        var cantidad = articuloFactura.cantidad;
        var precioBruto = precioUnitario - precioUnitario * descuentoGeneral / 100 - precioUnitario * articuloFactura.descuento / 100;

        var montoBruto = precioBruto * cantidad;
        var montoNeto = montoBruto / 1.21m; // Asumiendo que el IVA es del 21%
        var iva = montoBruto - montoNeto;
        articuloFactura.montoBruto = montoBruto;
        articuloFactura.montoNeto = montoNeto;
        articuloFactura.iva = iva;
    }
*/
// FUNCIONES AUXILIARES PARA CONVERSIONES ENTRE ENTIDADES Y DTOs


}
