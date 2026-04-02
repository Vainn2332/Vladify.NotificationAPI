using Microsoft.AspNetCore.Mvc;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Models;


namespace Vladify.NotificationAPI;

[Route("api/[controller]")]
[ApiController]
public class Test(INotificationService _notificationService) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<NotificationModel>> Get(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return _notificationService.GetAllAsync(pageNumber, pageSize, cancellationToken);
    }

    [HttpGet("{id}")]
    public Task<NotificationModel> Get(string id, CancellationToken cancellationToken = default)
    {
        return _notificationService.GetByIdAsync(id, cancellationToken)!;
    }

    [HttpPost]
    public Task<NotificationModel> Post([FromBody] NotificationRequestModel notificationRequestModel, CancellationToken cancellationToken = default)
    {
        return _notificationService.CreateAsync(notificationRequestModel, cancellationToken);
    }

    [HttpPut("{id}")]
    public Task<NotificationModel> Put(string id, [FromBody] NotificationModel notification, CancellationToken cancellationToken)
    {
        return _notificationService.UpdateAsync(notification, cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        await _notificationService.DeleteAsync(id, cancellationToken);
    }
}
