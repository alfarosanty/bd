using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    public  class Ingreso
        {
        public static String TABLA="INGRESO";
        
        public int Id { get; set; }
        
        public DateTime Fecha { get; set; }
        
        public Taller taller { get; set; }
        
        public List<ArticuloIngreso> Articulos { get; set; }

    }
