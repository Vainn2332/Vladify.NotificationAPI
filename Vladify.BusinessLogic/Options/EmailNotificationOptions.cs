namespace Vladify.BusinessLogic.Options;

public class EmailNotificationOptions
{
    public const string SectionName = "EmailNotificationOptions";

    public required string ApplicationPassword { get; set; }

    public int Port { get; set; }

    public required string SenderEmail { get; set; }

    public required string SenderName { get; set; }

    public required string SMTPClientUrl { get; set; }
}
