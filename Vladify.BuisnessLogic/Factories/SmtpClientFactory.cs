using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using System.Net;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Options;

namespace Vladify.BuisnessLogic.Factories;

public class SmtpClientFactory : ISmtpClientFactory
{
    private readonly EmailNotificationOptions _options;

    public SmtpClientFactory(IOptions<EmailNotificationOptions> options)
    {
        _options = options.Value;
    }

    public async Task<ISmtpClient> CreateClientAsync(CancellationToken cancellationToken)
    {
        var client = new SmtpClient();
        await client.ConnectAsync(_options.SMTPClientUrl, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
        var creds = new NetworkCredential(_options.SenderEmail, _options.ApplicationPassword);
        await client.AuthenticateAsync(creds, cancellationToken);

        return client;
    }

    public async Task EnsureConnectedAsync(ISmtpClient client, CancellationToken cancellationToken)
    {
        if (!client.IsConnected)
        {
            await client.ConnectAsync(_options.SMTPClientUrl, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
            var creds = new NetworkCredential(_options.SenderEmail, _options.ApplicationPassword);
            await client.AuthenticateAsync(creds, cancellationToken);
        }
    }
}
