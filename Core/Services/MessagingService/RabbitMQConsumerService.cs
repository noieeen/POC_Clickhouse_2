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
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMQConsumerService(IOptions<RabbitMQSetting> rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.HostName,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: RabbitMQQueues.OrderValidationQueue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Handle the message (you can deserialize it here if necessary)
            Console.WriteLine($"Received message: {message}");
            // Acknowledge message was processed
            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        _channel.BasicConsumeAsync(RabbitMQQueues.OrderValidationQueue, autoAck: false, consumer);
        return Task.CompletedTask;
    }

    public override async void Dispose()
    {
        await _channel?.CloseAsync();
        await _connection?.CloseAsync();
        base.Dispose();
    }
}