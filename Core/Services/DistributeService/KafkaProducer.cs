using Confluent.Kafka;
using Core.ServiceConfigs;
using Microsoft.Extensions.Options;

namespace Core.Services.DistributeService;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;
    private readonly KafkaSetting _settings;

    public KafkaProducer(IOptions<KafkaSetting> settings)
    {
        _settings = settings.Value;
        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers
        };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string message)
    {
        await _producer.ProduceAsync(_settings.Topic, new Message<Null, string> { Value = message });
    }
}