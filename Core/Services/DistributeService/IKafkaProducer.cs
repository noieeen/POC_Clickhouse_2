namespace Core.Services.DistributeService;

public interface IKafkaProducer
{
    Task ProduceAsync(string message);
}