namespace Core;

public class ConnectionStringProvider
{
    public string RedisConnectionString()
    {
        return "localhost:6379";
    }

    public string SQLServerConnectionString()
    {
        return "localhost:1433";
    }
}