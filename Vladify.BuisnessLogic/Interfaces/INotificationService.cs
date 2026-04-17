
using Vladify.BuisnessLogic.Models;

namespace Vladify.BuisnessLogic.Interfaces;

public interface INotificationService
{
    public Task<UserNotificationSettingsModel> CreateAsync(UserNotificationSettingsRequestModel UserNotificationSettingsRequestModel, CancellationToken cancellationToken);
    public Task<UserNotificationSettingsModel?> GetByIdAsync(string id, CancellationToken cancellationToken);
    public Task<IEnumerable<UserNotificationSettingsModel>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<IEnumerable<UserNotificationSettingsModel>> GetEmailSubscribersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<UserNotificationSettingsModel> UpdateAsync(UserNotificationSettingsModel UserNotificationSettingsModel, CancellationToken cancellationToken);
    public Task DeleteAsync(string id, CancellationToken cancellationToken);
}
