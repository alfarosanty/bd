using System.Text;
using BlumeAPI.servicios;

public class TemplateService:ITemplateService
{
    public string loadTemplate(string templateName)
    {
        var path = Path.Combine("Templates", templateName);
        return File.ReadAllText(path);
    }

    public string applyData(string html, Dictionary<string, string> data)
    {
        foreach (var kv in data)
            html = html.Replace("{{" + kv.Key + "}}", kv.Value);

        return html;
    }

public string renderDetalleArticulos(Dictionary<string, List<ArticuloFactura>> mapa)
{
    var sb = new StringBuilder();

    // mapa.Values = lista de listas
    foreach (var grupo in mapa.Values)
    {
        if (grupo.Count == 0)
            continue;

        var first = grupo[0];

        // Código
        var codigo = first.Codigo;

        // Descripción base: la del primer artículo
        var descripcion = first.Descripcion;

        // Agrego detalles tipo " (1 RO / 3 BL / 2 NE...)"
        var detalles = string.Join(" / ",
            grupo.Select(a =>
                $"{a.Cantidad} {a.Articulo.Color.Codigo}"
            )
        );

        descripcion += $" {detalles}";

        // Suma total de cantidades
        var cantidadTotal = grupo.Sum(a => a.Cantidad);

        // Precio unitario (todos del grupo tienen el mismo)
        var precioUnitario = first.PrecioUnitario;

        var importeBonificado = grupo.Sum(a => a.PrecioUnitario / 1.21m * a.Cantidad * (a.Descuento / 100m));

        // Subtotal
        var subtotal = cantidadTotal * (precioUnitario/1.21m) - importeBonificado;

        var iva = subtotal * 0.21m;

        var subtotalconIva = subtotal + iva;

        // Render de UNA sola fila por grupo
        sb.AppendLine($@"
            <tr>
                <td>{codigo}</td>
                <td>{descripcion}</td>
                <td>{cantidadTotal:N2}</td>
                <td>{precioUnitario:N2}</td>
                <td>{first.Descuento:N2}</td>
                <td>{subtotal:N2}</td>
                <td>{iva:N2}</td>
                <td>{subtotalconIva:N2}</td>

            </tr>
        ");
    }

    return sb.ToString();
}


}
