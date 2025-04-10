using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;

namespace Core.Services.MessagingService;

public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>
{
    private readonly RabbitMQSetting _rabbitMqSetting;
    private static readonly ActivitySource _activitySource = new("RabbitMQPublisher");

    // const string _queueName = "payment";
    // const string _exchangeName = "store.exchange";
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    public RabbitMQPublisher(IOptions<RabbitMQSetting> rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
    }

    public async Task PublishMessageAsync(T message, string queueName)
    {
        using var activity = _activitySource.StartActivity($"Publish to {queueName}", ActivityKind.Producer);

        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination.name", $"{RabbitMQExchange.OrderExchange}:{queueName}");
        activity?.SetTag("messaging.operation.name", "send");
        activity?.SetTag("messaging.operation.type", "send");
        activity?.SetTag("messaging.message.id", Guid.NewGuid());

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
        props.Headers = new Dictionary<string, object>();

        Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, (carrier, key, value) =>
        {
            carrier.Headers[key] = Encoding.UTF8.GetBytes(value);
        });

        // Publish message to the queue
        await channel.BasicPublishAsync("", queueName, false, props, body);
    }
}