using AutoMapper;
using Vladify.BusinessLogic.Exceptions;
using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Models;
using Vladify.DataAccess;
using Vladify.DataAccess.Entities;

namespace Vladify.BusinessLogic.Services;

public class NotificationService(INotificationRepository _repository, IMapper _mapper) : INotificationService
{
    public async Task<UserNotificationSettingsModel> CreateAsync(UserNotificationSettingsRequestModel UserNotificationSettingsRequestModel, CancellationToken cancellationToken)
    {
        var target = await _repository.GetByUserIdAsync(UserNotificationSettingsRequestModel.UserId, cancellationToken);
        if (target is not null)
        {
            throw new ArgumentException("Notification with such user already exists!");
        }
        var notification = _mapper.Map<UserNotificationSettings>(UserNotificationSettingsRequestModel);

        var newNotification = await _repository.CreateAsync(notification, cancellationToken);

        return _mapper.Map<UserNotificationSettingsModel>(newNotification);
    }

    public async Task<IEnumerable<UserNotificationSettingsModel>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetAllAsync(pageNumber, pageSize, cancellationToken);

        return _mapper.Map<IEnumerable<UserNotificationSettingsModel>>(notifications);
    }

    public async Task<UserNotificationSettingsModel?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(id, cancellationToken);

        return _mapper.Map<UserNotificationSettingsModel>(notification);
    }

    public async Task<UserNotificationSettingsModel> UpdateAsync(UserNotificationSettingsModel UserNotificationSettingsModel, CancellationToken cancellationToken)
    {
        var notification = _mapper.Map<UserNotificationSettings>(UserNotificationSettingsModel);

        await _repository.UpdateAsync(notification, cancellationToken);

        var newNotification = await _repository.GetByIdAsync(notification.Id, cancellationToken);

        return _mapper.Map<UserNotificationSettingsModel>(newNotification);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        _ = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Notification with such id doesn't exist!");

        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<UserNotificationSettingsModel>> GetEmailSubscribersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var subscribers = await _repository.GetEmailSubscribersAsync(pageNumber, pageSize, cancellationToken);

        return _mapper.Map<IEnumerable<UserNotificationSettingsModel>>(subscribers);
    }
}
