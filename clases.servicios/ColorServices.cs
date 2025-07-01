

using Npgsql;

public class ColorServices: BasicoServices 
{
    public override string getTabla()
    {
        return Color.TABLA;
    }

    public override Color readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }


public List<Color> listarColores(NpgsqlConnection conex)
{
    string commandText = GetSelect() + GetFromText();

    List<Color> colores = new List<Color>();
    using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
    {
        Console.WriteLine("Consulta: " + commandText);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                colores.Add(ReadColor(reader));
            }
        }
    }

    return colores;
}

private static string GetSelect()
{
    return "SELECT \"ID_COLOR\", \"CODIGO\", \"DESCRIPCION\", \"HEXA\" ";
}

private static string GetFromText()
{
    return "FROM \"COLOR\" ";
}

private static Color ReadColor(NpgsqlDataReader reader)
        {
            int? id = reader["ID_" + Color.TABLA] as int?;
            string codigo = reader["CODIGO"] as string;
            string descripcion = reader["DESCRIPCION"] as string;
            string colorHexa = reader["HEXA"] as string;
                

            return new Color
            {
                Id = id.Value,
                Codigo = codigo,
                Descripcion = descripcion,
                ColorHexa = colorHexa,

            };

            
        }



}