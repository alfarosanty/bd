namespace BlumeAPI.Entities;
    public  class Medida : Basico
    {
        public static String TABLA="MEDIDA";

    public int Id { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    
    }


