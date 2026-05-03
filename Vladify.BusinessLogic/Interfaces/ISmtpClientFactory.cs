using MailKit.Net.Smtp;

namespace Vladify.BusinessLogic.Interfaces;

public interface ISmtpClientFactory
{
    public Task<ISmtpClient> CreateClientAsync(CancellationToken cancellationToken);
    public Task EnsureConnectedAsync(ISmtpClient client, CancellationToken cancellationToken);
}
