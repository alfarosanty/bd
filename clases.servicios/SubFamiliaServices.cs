

using Npgsql;

public class SubFamiliaServices: BasicoServices 
{
    public override string getTabla()
    {
        return SubFamilia.TABLA;
    }

    public override SubFamilia readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }


public List<SubFamilia> listarSubFamilias(NpgsqlConnection conex)
{
    string commandText = GetSelect() + GetFromText();

    List<SubFamilia> subFamilias = new List<SubFamilia>();
    using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
    {
        Console.WriteLine("Consulta: " + commandText);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                subFamilias.Add(ReadSubFamilia(reader));
            }
        }
    }

    return subFamilias;
}

private static string GetSelect()
{
    return "SELECT \"ID_SUBFAMILIA\", \"CODIGO\", \"DESCRIPCION\"";
}

private static string GetFromText()
{
    return "FROM \"SUBFAMILIA\" ";
}

private static SubFamilia ReadSubFamilia(NpgsqlDataReader reader)
        {
            int? id = reader["ID_" + SubFamilia.TABLA] as int?;
            string codigo = reader["CODIGO"] as string;
            string descripcion = reader["DESCRIPCION"] as string;
                

            return new SubFamilia
            {
                Id = id.Value,
                Codigo = codigo,
                Descripcion = descripcion,

            };

            
        }



}