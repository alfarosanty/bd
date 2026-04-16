using BlumeAPI.Services;
using BlumeAPI.Repository;
using BlumeAPI.Entities;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

public class FacturaService : IFacturaService
{
private IFacturaRepository _facturaRepository;
private IArticuloRepository _articuloRepository;
private readonly AfipWsfeClient _afipClient;
private readonly AppDbContext _context;
private readonly IUnitOfWork _unitOfWork;
private readonly AfipSettings _afipSettings;
private readonly IARCAService _arcaService;


public FacturaService(IFacturaRepository facturaRepository, 
                            AfipWsfeClient afipClient, 
                            IArticuloRepository articuloRepository,
                            AppDbContext context,
                            IUnitOfWork unitOfWork,
                            IOptions<AfipSettings> afipSettings,
                            IARCAService arcaService
                            )
{
    _facturaRepository = facturaRepository;
    _afipClient = afipClient;
    _articuloRepository = articuloRepository;
    _context = context;
    _unitOfWork = unitOfWork;
    _afipSettings = afipSettings.Value;
    _arcaService = arcaService;
}


    public async Task<int> Crear(Factura factura, bool facturarEnArca)
    {
        Console.WriteLine("--- INICIO MÉTODO CREAR ---");

        if (factura.Articulos == null || !factura.Articulos.Any())
            throw new Exception("La factura no contiene artículos.");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Proceso ARCA (Independiente de la DB)
            if (facturarEnArca)
            {
                var loginTicket = await _arcaService.AutenticacionAsync("wsfe");
                var respuestaArca = await FacturarWsfeAsync(factura, loginTicket, long.Parse(_afipSettings.Cuit));

                if (!respuestaArca.Aprobado)
                    throw new Exception($"Error ARCA: {string.Join(" - ", respuestaArca.Errores)}");

                factura.CaeNumero = long.Parse(respuestaArca.Cae);
                factura.NumeroComprobante = int.Parse(respuestaArca.numeroComprobante);
                factura.FechaVencimientoCae = respuestaArca.CaeVencimiento;
            }


            await _unitOfWork.Articulos.DescontarStockAsync(factura.Articulos.ToList());
            Console.WriteLine("Stock descontado exitosamente vía Dapper.");

            foreach (var detalle in factura.Articulos)
            {
                detalle.IdArticulo = detalle.Articulo.Id;
                detalle.Articulo = null;
                detalle.Factura = factura;
            }

            if (factura.Cliente != null) 
            { 
                factura.IdCliente = factura.Cliente.Id; 
                factura.Cliente = null; 
            }

            // 4. Persistir Factura
            _unitOfWork.Context.ChangeTracker.Clear();

            // 1. Marcamos la factura como Added
            _unitOfWork.Context.Entry(factura).State = EntityState.Added;

            // 2. Marcamos explícitamente cada detalle como Added
            foreach (var detalle in factura.Articulos)
            {
                _unitOfWork.Context.Entry(detalle).State = EntityState.Added;
            }

            Console.WriteLine("Guardando factura y sus artículos en DB...");
            await _unitOfWork.SaveChangesAsync();
            
            await _unitOfWork.CommitAsync();
            
            Console.WriteLine($"Factura creada con ID: {factura.Id}");
            return factura.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--- ERROR DETECTADO ---: {ex.Message}");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<AfipResponse> FacturarWsfeAsync(
        Factura factura,
        LoginTicketResponseData loginTicket,
        long cuitRepresentada)
    {
        if (factura == null)
            throw new ArgumentNullException(nameof(factura));

        if (factura.Cliente == null)
            throw new Exception("La factura no tiene cliente asociado.");

        if(factura.PuntoDeVenta == null || factura.PuntoDeVenta == 0)
            throw new Exception("La factura no tiene punto de venta asignado.");

        if (factura.Articulos == null || !factura.Articulos.Any())
            throw new Exception("La factura no contiene artículos.");


        int tipoFactura = factura.TipoFactura switch
        {
            "A" => 1,
            "B" => 6,
            _ => throw new Exception("Tipo de factura no soportado.")
        };

        // 🔹 1. Obtener último comprobante
        var ultimoResult = await _afipClient.ConsultarUltimoAutorizadoAsync(
            loginTicket.Token,
            loginTicket.Sign,
            cuitRepresentada,
            factura.PuntoDeVenta!.Value,
            tipoFactura);

        if (!ultimoResult.Exitoso || ultimoResult.NumeroComprobante == null)
        {
            return new AfipResponse
            {
                Aprobado = false,
                idDocumento = factura.Id,
                Errores = ultimoResult.Errores?
                    .Select(e => $"{e.Codigo} - {e.Descripcion}")
                    .ToList() ?? new List<string> { "No se pudo obtener último comprobante." }
            };
        }

        long numeroComprobante = ultimoResult.NumeroComprobante.Value + 1;

        // 🔹 2. Cálculo AFIP

        var articulosAgrupados = AgruparPorCodigo(factura.Articulos);

        decimal totalNetoSinDescGeneral = 0m;

        foreach (var grupo in articulosAgrupados.Values)
        {
            var primero = grupo.First();

            if (primero.Descuento < 0 || primero.Descuento > 100)
                throw new Exception("Descuento por ítem inválido.");

            decimal precioNeto = Redondear(primero.PrecioUnitario / 1.21m); // le saco el IVA para trabajar con netos
            int cantidadTotal = grupo.Sum(a => a.Cantidad);

            decimal subtotalNeto = precioNeto * cantidadTotal;

            decimal descItem = Redondear(subtotalNeto * (primero.Descuento / 100m));

            decimal netoItem = Redondear(subtotalNeto - descItem);

            totalNetoSinDescGeneral += netoItem;
        }

        totalNetoSinDescGeneral = Redondear(totalNetoSinDescGeneral);

        // 🔹 Descuento general aplicado UNA sola vez
        decimal porcentajeDescGeneral = factura.DescuentoGeneral ?? 0;

        if (porcentajeDescGeneral < 0 || porcentajeDescGeneral > 100)
            throw new Exception("Descuento general inválido.");

        decimal descGeneral = Redondear(
            totalNetoSinDescGeneral * (porcentajeDescGeneral / 100m)
        );

        decimal totalGravado = Redondear(totalNetoSinDescGeneral - descGeneral);

        decimal totalIVA = factura.EximirIVA
            ? 0m
            : Redondear(totalGravado * 0.21m);

        decimal totalGeneral = Redondear(totalGravado + totalIVA);

        // Validación final AFIP
        if (Redondear(totalGravado + totalIVA) != totalGeneral)
            throw new Exception("Los totales no cierran correctamente.");

            // 🔹 3. Condición IVA receptor


        int tipoCondIVARec = factura.Cliente.CondicionFiscal?.Codigo switch
        {
            "RI" => 1,
            _ => 5,
            //"MO" => 6,
            //_ => throw new Exception("Condición IVA del cliente no reconocida.")
        };

        long nroDoc = long.Parse(
            factura.Cliente.Cuit.Replace("-", "")
        );

        // 🔹 4. Construir XML

        var builder = new ComprobanteCaeBuilderWsfe()
            .DatosFactura(
                tipoFactura,
                factura.PuntoDeVenta!.Value,
                (int)numeroComprobante,
                factura.FechaFactura)
            .Receptor(
                80,
                nroDoc,
                tipoCondIVARec)
            .Importes(
                totalGravado,
                totalGeneral);

        if (!factura.EximirIVA && totalIVA > 0)
        {
            builder.AgregarSubtotalIVA(new SubtotalIVA
            {
                codigo = 5,
                importe = totalIVA
            });
        }

        string xmlRequest = builder.Build();

        // 🔹 5. Enviar a AFIP

        var afipResponse = await _afipClient.AutorizarComprobanteAsync(
            loginTicket.Token,
            loginTicket.Sign,
            cuitRepresentada,
            xmlRequest,
            factura.Id);

        return afipResponse;
    }

    public async Task ActualizarDatosAFIPAsync(int idFactura, AfipResponse respuesta)
    {
            var factura = await _unitOfWork.Facturas.GetByIdAsync(idFactura);
        
         if (factura == null)
                throw new Exception($"No se encontró la factura con ID {idFactura}");
    
        _unitOfWork.Facturas.ActualizarDatosAFIP(factura, respuesta);

        await _unitOfWork.SaveChangesAsync();
    }

    public Task<Factura> GetByIdAsync(int idFactura)
    {
        var facturaResponse = _facturaRepository.GetByIdAsync(idFactura);
        return facturaResponse;
    }

    public async Task<PagedResult<Factura>> GetFacturasAsync(
        DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize)
    {
        // Ya no retorna List<>, retorna PagedResult<>
        return await _facturaRepository.GetAll(desde, hasta, facturadoARCA, page, pageSize);
    }

    public async Task<PagedResult<Factura>> GetFacturasByClienteAsync(
        int idCliente, DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize)
    {
        return await _facturaRepository.GetByCliente(idCliente, desde, hasta, facturadoARCA, page, pageSize);
    }


    public List<ArticuloResumen> ConstruirResumen(Dictionary<string, List<ArticuloFactura>> mapa)
    {
        var lista = new List<ArticuloResumen>();

        foreach (var kvp in mapa)
        {
            var codigo = kvp.Key;
            var articulos = kvp.Value;

            // descripción base (la del primer artículo)
            var descripcionBase = articulos.First().Descripcion;

            // ejemplo: "3 ROJO, 2 AZUL, 5 NEGRO"
            var detalles = articulos
                .GroupBy(a => a.Articulo.Color.Codigo)
                .Select(g => $"{g.Sum(x => x.Cantidad)} {g.Key}")
                .ToList();

            string descripcionFinal;

            if(articulos.First().Articulo.Codigo == "GEN")
                {
                    descripcionFinal = descripcionBase;
                }
            else
                descripcionFinal = descripcionBase + " - " + string.Join(", ", detalles);

            lista.Add(new ArticuloResumen
            {
                Codigo = codigo,
                Descripcion = descripcionFinal,
                Cantidad = articulos.Sum(a => a.Cantidad),
                PrecioUnitario = redondeoDecimales(articulos.First().PrecioUnitario),
                Descuento = (int)articulos.First().Descuento,
                Subtotal = redondeoDecimales(articulos.Sum(a => redondeoDecimales(a.PrecioUnitario * a.Cantidad) - redondeoDecimales(a.PrecioUnitario * a.Cantidad * (a.Descuento / 100m))) /1.21m),
                TotalDecimal = redondeoDecimales(articulos.Sum(a => redondeoDecimales(a.PrecioUnitario * a.Cantidad) - redondeoDecimales(a.PrecioUnitario * a.Cantidad * (a.Descuento / 100m)))),
                Iva = 21,
                Total = redondeoDecimales(articulos.Sum(a => redondeoDecimales(a.PrecioUnitario * a.Cantidad) - redondeoDecimales(a.PrecioUnitario * a.Cantidad * (a.Descuento / 100m))))
            });
        }

        return lista;
    }

    private decimal Redondear(decimal valor)
    {
        return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
    }

    private decimal redondeoDecimales(decimal valor)
    {
        return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
    }

    public Dictionary<string, List<ArticuloFactura>> AgruparPorCodigo(List<ArticuloFactura> articulos)
    {
        var mapa = new Dictionary<string, List<ArticuloFactura>>();

        foreach (var art in articulos)
        {
            if (!mapa.ContainsKey(art.Codigo))
            {
                mapa[art.Codigo] = new List<ArticuloFactura>();
            }

            mapa[art.Codigo].Add(art);
        }

        return mapa;
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
            facturaAsociada.PuntoDeVenta ?? 5,
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

            total += montoBruto ?? 0;
            ivaTotal += ivaArticulo ?? 0;
            gravado += montoNeto ?? 0;
        }

        // 6. Armar XML y autorizar
        var xml = new NotaCreditoBuilderWsfe()
            .DatosNotaCredito(tipoComprobante, facturaAsociada.PuntoDeVenta ?? 5, (int)numeroComprobante, DateTime.Today)
            .Receptor(tipoDoc: 80, nroDoc: long.Parse(facturaAsociada.Cliente.Cuit.Replace("-", "")), tipoCondIVAReceptor: tipoCondIVARec)
            .Importes(gravado: gravado, total: total)
            .AgregarSubtotalIVA(new SubtotalIVA { codigo = 5, importe = ivaTotal })
            .ComprobanteAsociado(
                tipo: tipoComprobanteAsociado,
                ptoVta: facturaAsociada.PuntoDeVenta ?? 5,
                nro: (long)(facturaAsociada.NumeroComprobante ?? 0),
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
