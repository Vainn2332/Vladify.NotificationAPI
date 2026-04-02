namespace Vladify.DataAccess.Options;

public class MongoDbOptions
{
    public const string SectionName = "MongoDbOptions";

    public required string DbName { get; set; }

    public required string ConnectionString { get; set; }
}
