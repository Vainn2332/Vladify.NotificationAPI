using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Vladify.BuisnessLogic.Fakers;
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
        private readonly UserSettingsSeeder _seeder;
        private readonly Stopwatch _timer = new Stopwatch();


        public TestController(IEmailService emailService, INotificationService notificationService, UserSettingsSeeder seeder)
        {
            _emailService = emailService;
            _notificationService = notificationService;
            _seeder = seeder;
        }

        [HttpPost("/send")]
        public async Task<long> Send(string message, string subject, CancellationToken cancellationToken)
        {
            _timer.Start();
            await _emailService.SendToAllUsersAsync(subject, message, cancellationToken);
            _timer.Stop();
            return _timer.ElapsedMilliseconds;
        }

        [HttpPost("/seed")]
        public Task Seed(int dataAmount, CancellationToken cancellationToken)
        {
            return _seeder.SeedAsync(dataAmount);
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
}