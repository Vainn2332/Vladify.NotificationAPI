namespace Vladify.BuisnessLogic.Interfaces;

public interface IEmailService
{
    public Task SendAsync(string receiver, string subject, string message, CancellationToken cancellationToken);
}
