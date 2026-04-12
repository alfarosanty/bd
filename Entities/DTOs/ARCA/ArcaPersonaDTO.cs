public class ArcaPersonaDto
{
    public string Cuit { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string Domicilio { get; set; } = "S/D";
    public string Localidad { get; set; } = "S/L";
    public string Provincia { get; set; } = "S/P";
    public string Telefono { get; set; } = "S/T";
    public string CondicionIva { get; set; } = "SIN DETERMINAR";
    public bool EsValido { get; set; }
}