namespace Vladify.BuisnessLogic.Interfaces;

public interface IKafkaProducerService
{
    public Task SendMessageAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken, string? key = null);
}
