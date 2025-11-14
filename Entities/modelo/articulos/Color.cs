
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BlumeAPI.Models{

    [Table("COLOR")]

public class Color{

    public static String TABLA="COLOR";


    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_COLOR")]
    public int Id {get; set;}

    [Column("CODIGO")]
    public string? Codigo {get; set;}

    [Column("DESCRIPCION")]
    public string? Descripcion {get; set;}
    
    [Column("HEXA")]
    public string? ColorHexa {get; set;}
    
}

}

/*

    public  class Color: Basico
    {
    }


*/