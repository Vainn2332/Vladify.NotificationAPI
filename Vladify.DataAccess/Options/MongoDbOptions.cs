using System.ComponentModel.DataAnnotations;

namespace Vladify.DataAccess.Options;

public class MongoDbOptions
{
    public const string SectionName = "MongoDbOptions";

    [Required]
    public required string DatabaseName { get; set; }

    [Required]
    public required string ConnectionString { get; set; }
}
