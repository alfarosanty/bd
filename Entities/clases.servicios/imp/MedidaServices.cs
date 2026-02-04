

using BlumeAPI.Entities.clases.modelo;
using Npgsql;

public class MedidaServices: BasicoServices 
{
    public override string getTabla()
    {
        return Medida.TABLA;
    }

    public override Medida readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }


public List<Medida> listarMedidas(NpgsqlConnection conex)
{
    string commandText = GetSelect() + GetFromText();

    List<Medida> medidas = new List<Medida>();
    using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
    {
        Console.WriteLine("Consulta: " + commandText);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                medidas.Add(ReadMedida(reader));
            }
        }
    }

    return medidas;
}

private static string GetSelect()
{
    return "SELECT \"ID_MEDIDA\", \"CODIGO\", \"DESCRIPCION\"";
}

private static string GetFromText()
{
    return "FROM \"MEDIDA\" ";
}

private static Medida ReadMedida(NpgsqlDataReader reader)
        {
            int? id = reader["ID_" + Medida.TABLA] as int?;
            string codigo = reader["CODIGO"] as string;
            string descripcion = reader["DESCRIPCION"] as string;
                

            return new Medida
            {
                Id = id.Value,
                Codigo = codigo,
                Descripcion = descripcion,

            };

            
        }



}