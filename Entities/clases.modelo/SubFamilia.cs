using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    public  class SubFamilia: Basico
    {
    public static String TABLA="SUBFAMILIA";

    public int? Id { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    }

