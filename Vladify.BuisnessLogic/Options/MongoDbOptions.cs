namespace Vladify.BuisnessLogic.Options;

public class MongoDbOptions
{
    public const string SectionName = "MongoDb";

    public required string ConnectionString { get; set; }

    public required string DbName { get; set; }

    public required string CollectionName { get; set; }
}
