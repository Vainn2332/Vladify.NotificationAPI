using Bogus;
using Vladify.BuisnessLogic.Models;

namespace Vladify.BuisnessLogic.Fakers;

public class UserSettingsFaker : Faker<UserNotificationSettingsRequestModel>
{
    public UserSettingsFaker()
    {
        RuleFor(property => property.UserId, setter => setter.Random.Guid());

        RuleFor(property => property.EmailAddress, setter => setter.Internet.Email());


        var subscriptionFaker = new Faker<NotificationSubscriptionModel>()
            .RuleFor(s => s.IsEmailSubscribed, f => f.Random.Bool())
            .RuleFor(s => s.IsPushSubscribed, f => f.Random.Bool());
        RuleFor(property => property.NotificationSubscription, setter => subscriptionFaker.Generate());
    }
}
