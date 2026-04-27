using MailKit.Net.Smtp;

namespace Vladify.BuisnessLogic.Interfaces;

public interface ISmtpClientFactory
{
    public Task<ISmtpClient> CreateClientAsync(CancellationToken cancellationToken);
    public Task CheckForConnectionAsync(ISmtpClient client, CancellationToken cancellationToken);
}
