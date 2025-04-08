namespace Core.Services.MessagingService;

public interface IRabbitMQConsumer
{
    public Task StartConsuming(string queueName);
}