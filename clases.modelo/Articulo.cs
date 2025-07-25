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
        public SubFamilia? SubFamilia{ get; set; }

        public ArticuloPrecio articuloPrecio{ get; set; }
        public int IdFabricante{ get; set; }

        public bool? Nuevo{ get; set; }
        
        public bool? Habilitado{ get; set; }

        public int? Stock{ get; set; }

        public int? CantidadEnCorte{ get; set; }

        public int? CantidadEnTaller{ get; set; }

    }

    public class ConsultaMedida{
        public string Medida{ get; set; }

        public int Cantidad{ get; set; }
    }

    public class ConsultaTallerCorte{
        public Articulo articulo{ get; set; }

        public int CantidadEnCorte{ get; set; }

        public int CantidadEnTaller{ get; set; }

    }

public class ConsultaTallerCortePorCodigo {
    public string Codigo { get; set; }
    public int CantidadEnCorteTotal { get; set; }
    public int CantidadEnTallerTotal { get; set; }
    public List<ConsultaTallerCorte> Consultas { get; set; } // ahora es una lista
}

