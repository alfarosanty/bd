using BlumeApi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlumeAPI.Models{

[Table("ARTICULO")]
public class Articulo
{
    public static string TABLA = "ARTICULO";

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_ARTICULO")]
    public int Id { get; set; }

    [Column("CODIGO")]
    public string Codigo { get; set; }

    [Column("DESCRIPCION")]
    public string Descripcion { get; set; }

    // --- Relaciones ---
    [Column("ID_COLOR")]
    public int? IdColor { get; set; }       // FK
    public Color? Color { get; set; }       // Navegación

    [Column("ID_MEDIDA")]
    public int? IdMedida { get; set; }
    public Medida? Medida { get; set; }

    [Column("ID_SUBFAMILIA")]
    public int? IdSubFamilia { get; set; }
    public SubFamilia? SubFamilia { get; set; }

    [Column("ID_ARTICULO_PRECIO")]
    public int? IdArticuloPrecio { get; set; }
    public ArticuloPrecio? ArticuloPrecio { get; set; }

    [Column("ID_FABRICANTE")]
    public int IdFabricante { get; set; }

    // --- Otros ---
    [Column("HABILITADO")]
    public bool? Habilitado { get; set; }

    [Column("STOCK")]
    public int? Stock { get; set; }

    // --- No están en la BD ---
    [NotMapped]
    public bool? Nuevo { get; set; }

    [NotMapped]
    public int? CantidadEnCorte { get; set; }

    [NotMapped]
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

}


