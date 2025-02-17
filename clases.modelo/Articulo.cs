using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    public  class Articulo
        {
        public static String TABLA="ARTICULO";
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public Color Color{ get; set; }
        public Medida Medida{ get; set; }
        public Familia Familia{ get; set; }

    }
