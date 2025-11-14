using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeAPI.Models;

namespace BlumeApi.Models
{
[Table("FACTURA")]
public class Factura
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_FACTURA")]
    public int Id { get; set; }

    [Column("FECHA_FACTURA")]
    public DateTime FechaFactura { get; set; }

    [Column("ID_CLIENTE")]
    public int IdCliente { get; set; }

    [ForeignKey(nameof(IdCliente))]
    public Cliente Cliente { get; set; } = null!;

    [Column("EXIMIR_IVA")]
    public bool EximirIVA { get; set; }

    [InverseProperty(nameof(ArticuloFactura.Factura))]
    public List<ArticuloFactura>? Articulos { get; set; }

    [Column("ID_PRESUPUESTO")]
    public int? IdPresupuesto { get; set; }

    [ForeignKey(nameof(IdPresupuesto))]
    public Presupuesto? Presupuesto { get; set; }

    [Column("IMPORTE_BRUTO")]
    public decimal? ImporteBruto { get; set; }

    [Column("PUNTO_DE_VENTA")]
    public int PuntoDeVenta { get; set; }

    [Column("NUMERO_FACTURA")]
    public int? NumeroFactura { get; set; }

    [Column("CAE_NUMERO")]
    public int? CaeNumero { get; set; }

    [Column("FECHA_VENCIMIENTO_CAE")]
    public DateTime? FechaVencimiento { get; set; }

    [Column("IMPORTE_NETO")]
    public decimal? ImporteNeto { get; set; }

    [Column("IVA")]
    public int? Iva { get; set; }

    [Column("TIPO_FACTURA")]
    public string TipoFactura { get; set; } = string.Empty;

    [Column("DESCUENTO")]
    public int? DescuentoGeneral { get; set; }
}

}
