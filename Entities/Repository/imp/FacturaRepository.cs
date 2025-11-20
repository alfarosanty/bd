using BlumeApi.Models;
using BlumeAPI.Models;
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class FacturaRepository : IFacturaRepository{
     private readonly AppDbContext _context;

    public FacturaRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> crear(Factura factura)
    {
        _context.Facturas.Add(factura);
        _context.SaveChangesAsync();
        return Task.FromResult(factura.Id);
    }

    public async Task<List<RespuestaEstadistica>> facturacionXCliente(DateTime fechaInicio, DateTime fechaFin)
{
    var resultado = await (from f in _context.Facturas
                           join c in _context.Clientes on f.Cliente.Id equals c.Id
                           join af in _context.ArticulosFactura on f.Id equals af.Factura.Id
                           where f.FechaFactura >= fechaInicio && f.FechaFactura <= fechaFin
                           group new { f, af } by new { c.Id, c.RazonSocial } into g
                           select new RespuestaEstadistica
                           {
                               Cliente = new Cliente
                               {
                                   Id = g.Key.Id,
                                   RazonSocial = g.Key.RazonSocial
                               },
                               Dinero = (int)g.Sum(x => x.f.ImporteBruto * (100 - (x.f.DescuentoGeneral ?? 0)) / 100),
                               CantidadArticulos = g.Sum(x => x.af.Cantidad)
                           }).ToListAsync();

    return resultado;
}

    public Task<List<Factura>> getFacturasPorFiltro(int? idCliente, string? tipoFactura, int? puntoDeVenta, DateTime fechaInicio, DateTime fechaFin)
    {
        throw new NotImplementedException(); //TODO: implementar esta logica
    }

    public Task CrearArticuloFacturaAsync(ArticuloFactura articuloFactura)
    {
        _context.ArticulosFactura.Add(articuloFactura);
        return _context.SaveChangesAsync();
    }
}