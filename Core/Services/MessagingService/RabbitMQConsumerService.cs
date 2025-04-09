using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;


namespace Core.Services.MessagingService;

public class RabbitMQConsumerService : BackgroundService
{
    private readonly RabbitMQSetting _rabbitMqSetting;

    public RabbitMQConsumerService(IOptions<RabbitMQSetting> rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.HostName,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password
        };

        // For ConnectionString
        // factory.Uri = new Uri("amqp://user:pass@hostName:port/vhost");

        using IConnection connection = await factory.CreateConnectionAsync();
        using IChannel channel = await connection.CreateChannelAsync();

        // await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: false,
        //     arguments: null);

        // Concurrent
        // Fair dispatch - don't give more than one message to a worker at a time
        // await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);


        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Handle the message (you can deserialize it here if necessary)
            Console.WriteLine($"Received message: {message}");
            // Acknowledge message was processed
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            // return Task.CompletedTask;
        };

        // await channel.BasicConsumeAsync(queueName, autoAck: true, consumer);
    }
}