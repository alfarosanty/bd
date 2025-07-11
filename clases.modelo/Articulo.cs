﻿using Npgsql;
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
        public SubFamilia? SubFamilia{ get; set; }

        public ArticuloPrecio articuloPrecio{ get; set; }
        public int IdFabricante{ get; set; }

        public bool? Nuevo{ get; set; }
        
        public bool? Habilitado{ get; set; }

        public int? Stock{ get; set; }

    }
