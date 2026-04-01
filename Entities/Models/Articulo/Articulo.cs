using BlumeAPI.Entities;


public class Articulo
{
    public static String TABLA="ARTICULO";
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;

    // --- IDs para el mapeo ---
    public int IdColor { get; set; } // Asegurate que se llame así
    public int IdMedida { get; set; }
    public int? IdSubFamilia { get; set; }
    public int? IdArticuloPrecio { get; set; } // OJO: en el JSON decía null pero el objeto venía lleno
    public int IdFabricante { get; set; }
    public int? IdAsociadoRelleno { get; set; }

    // --- Navegaciones ---
    public Color Color { get; set; } = null!;
    public Medida Medida { get; set; } = null!;
    public SubFamilia? SubFamilia { get; set; }
    
    public ArticuloPrecio? ArticuloPrecio { get; set; } 

    public bool? Habilitado { get; set; }
    public int? Stock { get; set; }

    // --- Ignorados ---
    public bool? Nuevo { get; set; }
    public int? CantidadEnCorte { get; set; }
    public int? CantidadEnTaller { get; set; }
}
    public class ConsultaMedida{
        public string Medida{ get; set; }

        public int Cantidad{ get; set; }
    }

    public class ConsultaTallerCorte{
        public Articulo articulo{ get; set; }

        public int CantidadEnCorteUnitario{ get; set; }
        public int CantidadEnTallerUnitario{ get; set; }
        public int CantidadSeparadoUnitario{ get; set; }
        public int CantidadEstanteriaUnitario{ get; set; }

        public int StockUnitario{ get; set; }


    }

public class ConsultaTallerCortePorCodigo {
    public string Codigo { get; set; }
    public int CantidadEnCorteTotal { get; set; }
    public int CantidadEnTallerTotal { get; set; }
    public int CantidadSeparadoTotal { get; set; }
    public int CantidadEstanteriaTotal { get; set; }
    public int StockTotal { get; set; }
    public List<ConsultaTallerCorte> Consultas { get; set; } // ahora es una lista
}

