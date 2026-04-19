namespace Vladify.BuisnessLogic.Options;

public class KafkaOptions
{
    public const string SectionName = "KafkaOptions";

    public required string ServerHost { get; set; }

    public required Topics Topics { get; set; }

    public required ConsumerGroups ConsumerGroups { get; set; }
}
