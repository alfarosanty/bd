using System.Reflection;
using System.Text.RegularExpressions;
using BlumeAPI.Entities;
using BlumeAPI.Repository;
using BlumeAPI.Services;
using ClosedXML.Excel;

class ArticuloServicesNUEVO : IArticuloService
{
    private readonly IArticuloRepository _articuloRepository;
    private readonly IUnitOfWork _unitOfWork;

    
    public ArticuloServicesNUEVO(IArticuloRepository articuloRepository, IUnitOfWork unitOfWork)
    {
        _articuloRepository = articuloRepository;
        _unitOfWork = unitOfWork;
    }
    
    // MÉTODOS NUEVOS

public async Task<Articulo?> GetArticulo(int idArticulo)
{
    var entity = await _articuloRepository
        .GetByIdAsync(idArticulo);

    if (entity == null)
        return null;

    return new Articulo
    {
        Id = entity.Id,
        Codigo = entity.Codigo,
        Descripcion = entity.Descripcion,
        Habilitado = entity.Habilitado,
        Stock = entity.Stock,

        Color = entity.Color == null ? null : new Color
        {
            Id = entity.Color.Id,
            Codigo = entity.Color.Codigo,
            Descripcion = entity.Color.Descripcion,
            ColorHexa = entity.Color.ColorHexa
        },

        Medida = entity.Medida == null ? null : new Medida
        {
            Id = entity.Medida.Id,
            Codigo = entity.Medida.Codigo,
            Descripcion = entity.Medida.Descripcion
        },

        SubFamilia = entity.SubFamilia == null ? null : new SubFamilia
        {
            Id = entity.SubFamilia.Id,
            Codigo = entity.SubFamilia.Codigo,
            Descripcion = entity.SubFamilia.Descripcion
        },

        ArticuloPrecio = entity.ArticuloPrecio == null ? null : new ArticuloPrecio
        {
            Id = entity.ArticuloPrecio.Id,
            Codigo = entity.ArticuloPrecio.Codigo,
            Descripcion = entity.ArticuloPrecio.Descripcion,
            Precio1 = entity.ArticuloPrecio.Precio1,
            Precio2 = entity.ArticuloPrecio.Precio2,
            Precio3 = entity.ArticuloPrecio.Precio3,
            Relleno = entity.ArticuloPrecio.Relleno
        }
    };
}


        public async Task<List<CartaKardexDTO>> GetFacturadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta)
    {
        var entities = await _articuloRepository
            .GetFacturadosByArticulo(idArticulo, desde, hasta);

        var articulo = await GetArticulo(idArticulo);

        entities.ForEach(carta => carta.articulo = articulo);
    
        return entities.OrderBy(e=>e.Fecha)
        .ToList();
    }


    public async Task<List<CartaKardexDTO>> GetIngresadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta)
    {
        var entities = await _articuloRepository
            .GetIngresadosByArticulo(idArticulo, desde, hasta);

        var articulo = await GetArticulo(idArticulo);

        entities.ForEach(carta => carta.articulo = articulo);
        return entities.OrderBy(e=>e.Fecha)
        .ToList();
    }

    public async Task<List<CartaKardexDTO>> GetResumenKardex(int idArticulo, DateTime? desde, DateTime? hasta)
    {
        var facturados = await GetFacturadosByArticulo(idArticulo, desde, hasta);
        var ingresados = await GetIngresadosByArticulo(idArticulo, desde, hasta);

        var resumen = new List<CartaKardexDTO>();

        resumen.AddRange(facturados);
        resumen.AddRange(ingresados);

        return resumen.OrderBy(r => r.Fecha).ToList();
    }

    public async Task<List<ArticuloPrecio>> GetArticulosPrecioAsync()
    {
        // Usamos el Unit of Work para acceder al repositorio de artículos
        return await _unitOfWork.Articulos.GetAllArticulosPrecioAsync(habilitados: null);
    }

    public async Task<byte[]> ExportarArticulosAExcel()
    {
        var articulos = await _unitOfWork.Articulos.GetAllAsync(habilitados: null);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("ARTICULOS");

            // --- LISTA DE PROPIEDADES A IGNORAR ---
            var ignorados = new[] { "Nuevo", "CantidadEnCorte", "CantidadEnTaller" };

            var propiedades = typeof(Articulo)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !ignorados.Contains(p.Name)) // <--- FILTRO DE EXCLUSIÓN
                .Where(p => p.PropertyType.IsPrimitive || 
                            p.PropertyType == typeof(string) || 
                            p.PropertyType == typeof(decimal) || 
                            p.PropertyType == typeof(decimal?) || // Importante para precios
                            p.PropertyType == typeof(int?) || 
                            p.PropertyType == typeof(bool?))
                .ToList();

            // Encabezados
            for (int i = 0; i < propiedades.Count; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                
                // Transformamos "IdColor" o "IdArticuloPrecio" a "ID_COLOR" o "ID_ARTICULO_PRECIO"
                string nombreFormateado = FormatearNombreEncabezado(propiedades[i].Name);
                
                cell.Value = nombreFormateado;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1f4e78");
                cell.Style.Font.FontColor = XLColor.White;
            }

            // Datos
            int filaIndex = 2;
            foreach (var art in articulos)
            {
                for (int colIndex = 0; colIndex < propiedades.Count; colIndex++)
                {
                    var valor = propiedades[colIndex].GetValue(art);
                    
                    // Manejo inteligente de tipos para que Excel los reconozca
                    var celda = worksheet.Cell(filaIndex, colIndex + 1);
                    
                    if (valor == null) {
                        celda.Value = "";
                    } else if (valor is bool b) {
                        celda.Value = b ? "SI" : "NO";
                    } else if (valor is decimal || valor is int || valor is double) {
                        celda.SetValue(Convert.ToDouble(valor)); // Para que sea numérico en Excel
                    } else {
                        celda.Value = valor.ToString();
                    }
                }
                filaIndex++;
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
    public async Task<byte[]> ExportarPreciosAExcel()
    {
        var precios = await _unitOfWork.Articulos.GetAllArticulosPrecioAsync(habilitados: null);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("PRECIOS");

            var propiedades = typeof(ArticuloPrecio)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsPrimitive || 
                            p.PropertyType == typeof(string) || 
                            p.PropertyType == typeof(decimal) ||
                            p.PropertyType == typeof(decimal?) ||
                            p.PropertyType == typeof(int?) || 
                            p.PropertyType == typeof(bool?))
                .ToList();

            for (int i = 0; i < propiedades.Count; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = propiedades[i].Name.ToUpper(); 
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#7b241c"); // Un color distinto (bordó) para diferenciar
                cell.Style.Font.FontColor = XLColor.White;
            }

            int filaIndex = 2;
            foreach (var p in precios)
            {
                for (int colIndex = 0; colIndex < propiedades.Count; colIndex++)
                {
                    var valor = propiedades[colIndex].GetValue(p);
                    // Si el valor es decimal (como un precio), ClosedXML lo maneja mejor así:
                    if (valor is decimal d) 
                        worksheet.Cell(filaIndex, colIndex + 1).Value = d;
                    else
                        worksheet.Cell(filaIndex, colIndex + 1).Value = valor?.ToString() ?? "";
                }
                filaIndex++;
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }




// AUXILIAR

    private string FormatearNombreEncabezado(string nombre)
    {
        // 1. Insertamos un guion bajo antes de cada mayúscula, excepto la primera
        // Ej: IdColor -> Id_Color
        var resultado = Regex.Replace(nombre, @"(\p{Ll})(\p{Lu})", "$1_$2");
        
        // 2. Pasamos todo a mayúsculas
        return resultado.ToUpper();
    }

}