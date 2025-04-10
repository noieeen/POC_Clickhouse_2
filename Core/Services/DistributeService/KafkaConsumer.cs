using Confluent.Kafka;
using Core.ServiceConfigs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.DistributeService;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private CancellationToken _cancellationToken;

    private readonly KafkaSetting _settings;
    private IConsumer<Ignore, string>? _consumer;

    public KafkaConsumer(IOptions<KafkaSetting> settings, ILogger<KafkaConsumer> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting KafkaConsumer...");

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = "consumer-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(_settings.Topic);

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;
        if (_consumer == null)
        {
            _logger.LogError("Kafka consumer not initialized. Aborting consumption.");
            return Task.CompletedTask;
        }


        return Task.Run(() =>
        {
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var cr = _consumer.Consume(_cancellationToken);
                    if (cr != null)
                    {
                        _logger.LogInformation("Consumed message: {Message}", cr.Message.Value);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer operation was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka consumer encountered an error.");
            }
            finally
            {
                _consumer?.Close();
            }
        }, stoppingToken);
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}