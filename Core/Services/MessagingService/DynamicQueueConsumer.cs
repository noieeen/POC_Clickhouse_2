using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Core.Services.MessagingService;

public class DynamicQueueConsumer : IDynamicQueueConsumer
{
    private readonly RabbitMQSetting _settings;
    private IConnection? _connection;
    private readonly Dictionary<string, IChannel> _channels = new();

    public DynamicQueueConsumer(IOptions<RabbitMQSetting> settings)
    {
        _settings = settings.Value;
    }

    public async Task ConsumeQueueAsync(string queueName)
    {
        if (_channels.ContainsKey(queueName)) return;

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection ??= await factory.CreateConnectionAsync();
        var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queueName, false, false, false, null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[x] Received on {queueName}: {message}");
        };

        await channel.BasicConsumeAsync(queueName, true, consumer);
        _channels[queueName] = channel;
    }

    public Task CloseQueueAsync(string queueName)
    {
        if (_channels.TryGetValue(queueName, out var channel))
        {
            channel.CloseAsync();
            channel.Dispose();
            _channels.Remove(queueName);
        }

        if (_channels.Count == 0 && _connection != null)
        {
            _connection.CloseAsync();
            _connection.Dispose();
            _connection = null;
        }

        return Task.CompletedTask;
    }

    public Task AddQueueConsumerAsync(string queueName) => ConsumeQueueAsync(queueName);

    public Task RemoveQueueConsumerAsync(string queueName) => CloseQueueAsync(queueName);

    public IEnumerable<string> ListActiveQueues() => _channels.Keys;
}