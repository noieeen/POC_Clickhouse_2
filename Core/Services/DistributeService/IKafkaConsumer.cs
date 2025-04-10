namespace Core.Services.DistributeService;

public interface IKafkaConsumer
{
    Task ConsumeAsync(string topic, CancellationToken cancellationToken = default);

}