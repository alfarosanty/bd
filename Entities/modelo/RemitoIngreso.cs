using BlumeApi.Models;


    public  class RemitoIngreso
    {
        public static String TABLA="INGRESO";

        public Taller Taller { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; }

        public List<ArticuloIngreso> Articulos { get; set; }

        public int Id { get; set; }

    
    }


