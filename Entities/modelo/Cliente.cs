using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlumeAPI.Models{
[Table("CLIENTE")]
public class Cliente
{
    public static string TABLA = "CLIENTE";

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_CLIENTE")]
    public int Id { get; set; }

    [Column("RAZON_SOCIAL")]
    public string RazonSocial { get; set; }

    [Column("CONTACTO")]
    public string? Contacto { get; set; }

    [Column("TELEFONO")]
    public string? Telefono { get; set; }

    [Column("DOMICILIO")]
    public string? Domicilio { get; set; }

    [Column("LOCALIDAD")]
    public string? Localidad { get; set; }

    [Column("PROVINCIA")]
    public string? Provincia { get; set; }

    [Column("CUIT")]
    public string? Cuit { get; set; }

    [Column("TRANSPORTE")]
    public string? Transporte { get; set; }

    // 👇 Foreign Key
    [Column("ID_CONDICION_FISCAL")]
    public int? IdCondicionFiscal { get; set; }

    // 👇 Propiedad de navegación
    [ForeignKey("IdCondicionFiscal")]
    public CondicionFiscal? CondicionFiscal { get; set; }
}


}


