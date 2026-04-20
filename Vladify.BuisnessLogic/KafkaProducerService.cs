using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Options;

namespace Vladify.BuisnessLogic;

public class KafkaProducerService : IKafkaProducerService
{
    private IProducer<string, string> _producer;
    private readonly KafkaConsumerOptions _options;

    public KafkaProducerService(IOptions<KafkaConsumerOptions> options)
    {
        _options = options.Value;

        var config = new ProducerConfig()
        {
            BootstrapServers = _options.ServerHost
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    public Task SendMessageAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken, string? key = null)
    {
        var jsonMessage = JsonSerializer.Serialize(message);

        return _producer.ProduceAsync(topic, new Message<string, string>()
        {
            Key = key!,
            Value = jsonMessage
        }, cancellationToken);
    }
}
