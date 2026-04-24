using Vladify.BuisnessLogic.Interfaces;

namespace Vladify.BuisnessLogic.Fakers;

public class UserSettingsSeeder(INotificationService _notificationService)
{
    public async Task SeedAsync(int dataAmount)
    {
        var data = new UserSettingsFaker().Generate(dataAmount);
        for (int i = 0; i < dataAmount; i++)
        {
            await _notificationService.CreateAsync(data[i], CancellationToken.None);
        }
    }
}
