using Microsoft.AspNetCore.Mvc;
using Vladify.DataAccess;
using Vladify.DataAccess.Entities;


namespace Vladify.NotificationAPI;

[Route("api/[controller]")]
[ApiController]
public class Test(INotificationRepository _repo) : ControllerBase
{
    [HttpGet]
    public Task<List<NotificationInfo>> Get(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return _repo.GetAllAsync(pageNumber, pageSize, cancellationToken);
    }

    [HttpGet("{id}")]
    public Task<NotificationInfo> Get(string id, CancellationToken cancellationToken = default)
    {
        return _repo.GetByIdAsync(id, cancellationToken)!;
    }

    [HttpPost]
    public Task<NotificationInfo> Post([FromBody] NotificationInfo notification, CancellationToken cancellationToken = default)
    {
        return _repo.CreateAsync(notification, cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task Put(string id, [FromBody] NotificationInfo notification, CancellationToken cancellationToken)
    {
        await _repo.UpdateAsync(notification, cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task Delete(string id, CancellationToken cancellationToken)
    {
        await _repo.DeleteAsync(id, cancellationToken);
    }
}
