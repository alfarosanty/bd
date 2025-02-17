

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
}