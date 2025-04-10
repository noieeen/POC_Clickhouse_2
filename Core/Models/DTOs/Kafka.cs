namespace Core.Models.DTOs;

public class Kafka
{
    public string EventType { get; set; }
    public string Payload { get; set; }
    public DateTime Timestamp { get; set; }
}