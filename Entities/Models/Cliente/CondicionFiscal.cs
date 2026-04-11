using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public  class CondicionFiscal
{
    public static String TABLA="CONDICION_AFIP";
    public static String ID="ID_CONDICION";
    public int IdCondicion { get; set; }
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;

}


