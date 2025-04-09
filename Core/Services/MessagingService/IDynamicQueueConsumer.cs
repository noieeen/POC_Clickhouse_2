namespace Core.Services.MessagingService;

public interface IDynamicQueueConsumer
{
    Task ConsumeQueueAsync(string queueName);
    Task CloseQueueAsync(string queueName);
    Task AddQueueConsumerAsync(string queueName);
    Task RemoveQueueConsumerAsync(string queueName);
    IEnumerable<string> ListActiveQueues();
}