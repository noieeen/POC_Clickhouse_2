using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Core.Services.MessagingService;

public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>
{
    private readonly RabbitMQSetting _rabbitMqSetting;

    public RabbitMQPublisher(IOptions<RabbitMQSetting> rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
    }

    public async Task PublishMessageAsync(T message, string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.HostName,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password
        };

        using IConnection connection = await factory.CreateConnectionAsync();
        using IChannel channel = await connection.CreateChannelAsync();

        // Declare queue if not exists
        await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var messageJson = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(messageJson);
        var props = new BasicProperties();
        // Publish message to the queue

        await channel.BasicPublishAsync("exchangeName", "routingKey", false, props, body);
    }
}