namespace Core.Services.MessagingService;

public interface IRabbitMQPublisher<T>
{
    Task PublishMessageAsync(T message, string queueName);
}