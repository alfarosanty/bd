public static class AfipHelper
{
    public static string ObtenerNombreProvincia(int id) => id switch
    {
        0 => "CIUDAD AUTÓNOMA DE BUENOS AIRES",
        1 => "BUENOS AIRES",
        2 => "CATAMARCA",
        3 => "CÓRDOBA",
        4 => "CORRIENTES",
        5 => "ENTRE RÍOS",
        6 => "JUJUY",
        7 => "MENDOZA",
        8 => "LA RIOJA",
        9 => "SALTA",
        10 => "SAN JUAN",
        11 => "SAN LUIS",
        12 => "SANTA FE",
        13 => "SANTIAGO DEL ESTERO",
        14 => "TUCUMÁN",
        15 => "CHACO",
        16 => "CHUBUT",
        17 => "FORMOSA",
        18 => "MISIONES",
        19 => "NEUQUÉN",
        20 => "LA PAMPA",
        21 => "RÍO NEGRO",
        22 => "SANTA CRUZ",
        23 => "TIERRA DEL FUEGO",
        _ => "PROVINCIA DESCONOCIDA"
    };
}