using BlumeAPI.Data.Entities;
using BlumeAPI.Services;
using BlumeAPI.Repository;
using BlumeAPI.Entities.clases.modelo;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

public class FacturaServicesNUEVO : IFacturaService
{
private IFacturaRepository _facturaRepository;
private IArticuloRepository _articuloRepository;
private readonly AfipWsfeClient _afipClient;
private readonly AppDbContext _context;

public FacturaServicesNUEVO(IFacturaRepository facturaRepository, 
                            AfipWsfeClient afipClient, 
                            IArticuloRepository articuloRepository,
                            AppDbContext context
                            )
{
    _facturaRepository = facturaRepository;
    _afipClient = afipClient;
    _articuloRepository = articuloRepository;
    _context = context;
}

public Task<FacturaEntity> GetByIdAsync(int idFactura)
{
    var facturaResponse = _facturaRepository.GetByIdAsync(idFactura);
    return facturaResponse;
}

//================================ NOTA DE CRÉDITO ================================
public async Task<NotaDeCredito> CrearNotaCreditoAsync(NotaDeCredito notaDeCredito)
{
    try
    {
        // Insertar cabecera
        int id = await _facturaRepository.InsertarNotaCreditoAsync(notaDeCredito);
        notaDeCredito.Id = id;

        // Insertar artículos vinculados al id recién generado
        if (notaDeCredito.Articulos != null && notaDeCredito.Articulos.Any())
        {
            foreach (var art in notaDeCredito.Articulos)
                art.IdNotaDeCredito = id;

            await _facturaRepository.InsertarArticulosNotaCreditoAsync(notaDeCredito.Articulos);
            
            var conn = (_context.Database.GetDbConnection() as NpgsqlConnection)!;
            var tran = (_context.Database.CurrentTransaction?.GetDbTransaction() as NpgsqlTransaction)!;
            await _articuloRepository.RestaurarStockAsync(
                notaDeCredito.Articulos.Cast<IArticuloConStock>().ToList(), 
                conn, 
                tran);
        }

        return notaDeCredito;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al crear la nota de crédito: {ex.Message}");
        throw;
    }
}

public async Task<AfipResponse?> ValidarNotaCreditoWsfeAsync
(NotaDeCredito notaDeCredito, 
LoginTicketResponseData loginTicket, 
long cuitRepresentada)
{
    try
    {
        // 1. Obtener factura asociada
        var facturaAsociada = await _facturaRepository.GetByIdAsync(notaDeCredito.IdFacturaAsociada);
        if (facturaAsociada == null)
            return new AfipResponse
            {
                Aprobado = false,
                Errores = new List<string> { $"Factura asociada con ID {notaDeCredito.IdFacturaAsociada} no encontrada." }
            };

        // 2. Determinar tipo de comprobante
        int tipoComprobante;
        int tipoComprobanteAsociado;

        switch (facturaAsociada.TipoFactura)
        {
            case "A":
                tipoComprobante = NotaCreditoBuilderWsfe.Tipos.NotaCreditoA;
                tipoComprobanteAsociado = 1;
                break;
            case "B":
                tipoComprobante = NotaCreditoBuilderWsfe.Tipos.NotaCreditoB;
                tipoComprobanteAsociado = 6;
                break;
            default:
                throw new Exception($"Tipo de factura no soportado: {facturaAsociada.TipoFactura}");
        }

        // 3. Obtener próximo número de comprobante
        var ultimoResult = await _afipClient.ConsultarUltimoAutorizadoAsync(
            loginTicket.Token,
            loginTicket.Sign,
            cuitRepresentada,
            facturaAsociada.PuntoDeVenta,
            tipoComprobante);

        if (!ultimoResult.Exitoso || ultimoResult.NumeroComprobante == null)
            return new AfipResponse
            {
                Aprobado = false,
                Errores = ultimoResult.Errores?
                    .Select(e => $"{e.Codigo} - {e.Descripcion}")
                    .ToList() ?? new List<string> { "No se pudo obtener último comprobante." }
            };

        long numeroComprobante = ultimoResult.NumeroComprobante.Value + 1;

        // 4. Condición IVA receptor
        int tipoCondIVARec = facturaAsociada.Cliente.IdCondicionFiscal switch
        {
            1/*"RI"*/ => 1,
            _ => 5
        };

        // 5. Calcular montos
        decimal total = 0, gravado = 0, ivaTotal = 0;
        var descuentoGeneral = facturaAsociada.DescuentoGeneral;

        foreach (var articulo in notaDeCredito.Articulos!)
        {
            var precioBruto = articulo.PrecioUnitario
                - articulo.PrecioUnitario * descuentoGeneral / 100
                - articulo.PrecioUnitario * articulo.Descuento / 100;

            var montoBruto = precioBruto * articulo.Cantidad;
            var montoNeto = montoBruto / 1.21m;
            var ivaArticulo = montoBruto - montoNeto;

            total += montoBruto;
            ivaTotal += ivaArticulo;
            gravado += montoNeto;
        }

        // 6. Armar XML y autorizar
        var xml = new NotaCreditoBuilderWsfe()
            .DatosNotaCredito(tipoComprobante, facturaAsociada.PuntoDeVenta, (int)numeroComprobante, DateTime.Today)
            .Receptor(tipoDoc: 80, nroDoc: long.Parse(facturaAsociada.Cliente.Cuit.Replace("-", "")), tipoCondIVAReceptor: tipoCondIVARec)
            .Importes(gravado: gravado, total: total)
            .AgregarSubtotalIVA(new SubtotalIVA { codigo = 5, importe = ivaTotal })
            .ComprobanteAsociado(
                tipo: tipoComprobanteAsociado,
                ptoVta: facturaAsociada.PuntoDeVenta,
                nro: facturaAsociada.NumeroComprobante,
                cuit: long.Parse(facturaAsociada.Cliente.Cuit.Replace("-", "")),
                fecha: facturaAsociada.FechaFactura)
            .Build();

        // 7. Enviar a ARCA
        return await _afipClient.AutorizarComprobanteAsync(
            loginTicket.Token,
            loginTicket.Sign,
            cuitRepresentada,
            xml,
            notaDeCredito.Id);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al validar nota de crédito: {ex.Message}");
        throw;
    }
}

public async Task ActualizarNotaCreditoDatosAfipAsync(int idInterno, AfipResponse respuestaAfip)
{
    try
    {
        await _facturaRepository.ActualizarNotaDeCreditoAfipAsync(idInterno, respuestaAfip);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al actualizar datos AFIP de la nota de crédito: {ex.Message}");
        throw;
    }
}


}
