using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlumeAPI.Models;

namespace BlumeAPI
{
    [Table("USUARIO")]

public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_USUARIO")] // columna exacta
    public int Id { get; set; }

    [Column("NOMBRE")]
    public string Nombre { get; set; }

    [Column("APELLIDO")]
    public string Apellido { get; set; }

    [Column("USERNAME")]
    public string UserName { get; set; }

    [Column("CONTRASENIA")]
    public string Contrasenia { get; set; }

    [Column("ROL")]
    public Rol Rol { get; set; }
}
}