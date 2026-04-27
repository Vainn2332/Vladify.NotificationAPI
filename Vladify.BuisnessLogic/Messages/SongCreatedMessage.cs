namespace Vladify.BuisnessLogic.Messages;

public class SongCreatedMessage
{
    public required string Title { get; set; }

    public required string Album { get; set; }

    public required string Author { get; set; }
}
