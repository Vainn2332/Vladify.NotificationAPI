namespace Vladify.BuisnessLogic.Options;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMqOptions";

    public required string ServerHost { get; set; }

    public required Queues Queues { get; set; }
}
