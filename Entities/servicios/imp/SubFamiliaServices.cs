

using BlumeApi.Models;
using BlumeAPI.Repository;
using BlumeAPI.Services;
using Npgsql;

public class SubFamiliaServices: ISubFamiliaService
{
    
    private readonly ISubFamiliaRepository isubFamiliaRepository;
    
    public SubFamiliaServices(ISubFamiliaRepository subFamiliaRepository)
    {
        isubFamiliaRepository = subFamiliaRepository;
    }

    public async Task<List<SubFamilia>> listarSubFamiliasAsync()
    {
        return await isubFamiliaRepository.listarSubFamiliasAsync();
    }

    /*
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
            string codigo = reader.GetString(reader.GetOrdinal("CODIGO"));
            string descripcion = reader.GetString(reader.GetOrdinal("DESCRIPCION"));
            

            return new SubFamilia
            {
                Id = id.Value,
                Codigo = codigo,
                Descripcion = descripcion,

            };

            
        }


*/
}