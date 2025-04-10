namespace Core.ServiceConfigs;

public class KafkaSetting
{
    public string BootstrapServers { get; set; } = "";
    public string Topic { get; set; } = "";
}