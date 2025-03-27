namespace Core;

public class ConnectionStringProvider
{
    public string RedisConnectionString()
    {
        return "localhost:6379";
    }

    public string SQLServerConnectionString()
    {
        return "Server=mssql;Database=Mock_Monitoring_DB;User Id=sa;Password=MyPass@word90_;Encrypt=False;";
    }
}