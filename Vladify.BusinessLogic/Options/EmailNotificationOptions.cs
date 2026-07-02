using System.ComponentModel.DataAnnotations;

namespace Vladify.BusinessLogic.Options;

public class EmailNotificationOptions
{
    public const string SectionName = "EmailNotificationOptions";

    [Required]
    public required string ApplicationPassword { get; set; }

    [Required]
    public int Port { get; set; }

    [Required]
    public required string SenderEmail { get; set; }

    [Required]
    public required string SenderName { get; set; }

    [Required]
    public required string SMTPClientUrl { get; set; }
}
