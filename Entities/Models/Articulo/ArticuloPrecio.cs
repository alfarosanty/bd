using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class ArticuloPrecio
    {
        // Esta constante te sirve si hacés consultas manuales o para referencia
        public static string TABLA = "ARTICULO_PRECIO";

        public int Id { get; set; }
        
        public string? Codigo { get; set; }
        
        public string? Descripcion { get; set; }

        public decimal? Precio1 { get; set; }
        public decimal? Precio2 { get; set; }
        public decimal? Precio3 { get; set; }

        public int? Relleno { get; set; }


    }