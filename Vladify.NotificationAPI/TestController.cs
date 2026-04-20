using Microsoft.AspNetCore.Mvc;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vladify.NotificationAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(KafkaProducer producer, INotificationService _notificationService) : ControllerBase
    {

        [HttpPost("/testKafka")]
        public void Post()
        {
            producer.CreateMessageAsync();
        }

        [HttpPost]
        public Task<UserNotificationSettingsModel> Post([FromBody] UserNotificationSettingsRequestModel userNotificationSettingsRequestModel, CancellationToken cancellationToken = default)
        {
            return _notificationService.CreateAsync(userNotificationSettingsRequestModel, cancellationToken);
        }

        [HttpGet]
        public Task<IEnumerable<UserNotificationSettingsModel>> Get(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return _notificationService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
