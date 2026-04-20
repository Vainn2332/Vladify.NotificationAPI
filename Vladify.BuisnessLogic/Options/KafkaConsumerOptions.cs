namespace Vladify.BuisnessLogic.Options;

public class KafkaConsumerOptions
{
    public const string SectionName = "KafkaOptions";

    public required string ServerHost { get; set; }

    public required string ConsumerGroupName { get; set; }

    public required List<string> Topics { get; set; }
}
