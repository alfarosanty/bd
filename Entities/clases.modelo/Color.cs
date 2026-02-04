using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BlumeAPI.Entities.clases.modelo;


    public  class Color: Basico
    {
        public static String TABLA="COLOR";
        public int Id {get; set;}

        public string? Codigo {get; set;}

        public string? Descripcion {get; set;}


        public string? ColorHexa {get; set;}
    }


