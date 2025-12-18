namespace BlumeAPI.servicios{

public interface ITemplateService{
    public string loadTemplate(string templateName);
    public string applyData(string html, Dictionary<string, string> data);
    public string renderDetalleArticulos(Dictionary<string, List<ArticuloFactura>> data);
}
}