namespace Vladify.BuisnessLogic.Interfaces;

public interface IEmailService
{
    public Task SendToAllUsersAsync(string subject, string message, CancellationToken cancellationToken);
}
