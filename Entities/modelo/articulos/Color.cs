using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlumeAPI{

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