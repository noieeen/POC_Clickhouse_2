using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;


namespace Core.Services.MessagingService;

public class RabbitMQConsumer : IRabbitMQConsumer
{
    private readonly RabbitMQSetting _rabbitMqSetting;

    public RabbitMQConsumer(IOptions<RabbitMQSetting> rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
    }

    public async Task StartConsuming(string queueName)
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

        await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Handle the message (you can deserialize it here if necessary)
            Console.WriteLine($"Received message: {message}");
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer);
    }
}