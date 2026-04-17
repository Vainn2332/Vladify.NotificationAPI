using Microsoft.AspNetCore.Mvc;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Models;

namespace Vladify.NotificationAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public TestController(IEmailService emailService, INotificationService notificationService)
        {
            _emailService = emailService;
            _notificationService = notificationService;
        }

        [HttpPost("/send")]
        public Task Send(string message, string subject, CancellationToken cancellationToken)
        {
            return _emailService.SendToAllUsersAsync(subject, message, cancellationToken);
        }



        [HttpGet]
        public Task<IEnumerable<UserNotificationSettingsModel>> Get(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return _notificationService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        }

        [HttpGet("{id}")]
        public Task<UserNotificationSettingsModel> Get(string id, CancellationToken cancellationToken = default)
        {
            return _notificationService.GetByIdAsync(id, cancellationToken)!;
        }

        [HttpPost]
        public Task<UserNotificationSettingsModel> Post([FromBody] UserNotificationSettingsRequestModel notificationRequestModel, CancellationToken cancellationToken = default)
        {
            return _notificationService.CreateAsync(notificationRequestModel, cancellationToken);
        }
    }
}