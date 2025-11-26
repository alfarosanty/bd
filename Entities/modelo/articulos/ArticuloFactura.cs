using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeAPI.Models;

namespace BlumeApi.Models
{
    [Table("ARTICULO_FACTURA")]
    public class ArticuloFactura
    {
        public static string TABLA = "ARTICULO_FACTURA";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_ARTICULO_FACTURA")]
        public int Id { get; set; }

        [Column("ID_ARTICULO")]
        [ForeignKey(nameof(Articulo))]
        public int IdArticulo { get; set; }
        public Articulo Articulo { get; set; } = null!;

        [Column("ID_FACTURA")]
        [ForeignKey(nameof(Factura))]
        public int? IdFactura { get; set; }

        [ForeignKey(nameof(IdFactura))]
        public Factura? Factura { get; set; }

        [Column("CANTIDAD")]
        public int Cantidad { get; set; }

        [Column("PRECIO_UNITARIO", TypeName = "numeric(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column("DESCUENTO", TypeName = "numeric(5,2)")]
        public decimal Descuento { get; set; }

        [Column("CODIGO")]
        public string Codigo { get; set; } = string.Empty;

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
