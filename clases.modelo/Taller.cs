using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public  class Taller: Basico
    {
        public static String TABLA="FABRICANTE";

        public int Id { get; set; }
        public string telefono { get; set; }
        public string razonSocial {get;set;}

        public string direccion {get;set;}

        public string provincia {get;set;}
    }


