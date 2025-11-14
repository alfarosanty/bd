using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlumeAPI.Models{
    
    [Table("CONDICION_AFIP")]
    public  class CondicionFiscal
    {
    
    public static String TABLA="CONDICION_AFIP";
    [Column("ID_CONDICION")]
    public int Id { get; set; }

     [Column("CODIGO")]
    public string? Codigo { get; set; }

     [Column("DESCRIPCION")]
    public string? Descripcion { get; set; }

    }

}


