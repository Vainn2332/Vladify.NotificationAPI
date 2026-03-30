using Microsoft.AspNetCore.Mvc;
using Vladify.DataAccess;
using Vladify.DataAccess.Entities;


namespace Vladify.NotificationAPI;

[Route("api/[controller]")]
[ApiController]
public class Test(INotificationRepository _repo) : ControllerBase
{


    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
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
    public void Put(int id, [FromBody] string value)
    {
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
