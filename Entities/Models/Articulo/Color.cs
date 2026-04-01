namespace BlumeAPI.Entities;


    public  class Color: Basico
    {
        public static String TABLA="COLOR";
    public int Id { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    public string? ColorHexa { get; set; }
    }


