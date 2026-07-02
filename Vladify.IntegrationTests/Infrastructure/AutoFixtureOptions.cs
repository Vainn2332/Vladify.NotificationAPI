using AutoFixture;
using MongoDB.Bson;
using System.Net.Mail;
using Vladify.DataAccess.Entities;

namespace Vladify.IntegrationTests.Infrastructure;

public static class AutoFixtureOptions
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();

        fixture.Customize<UserNotificationSettings>(builder => builder
            .With(s => s.Id, () => ObjectId.GenerateNewId().ToString())
            .With(x => x.EmailAddress, () => fixture.Create<MailAddress>().Address)
        );

        return fixture;
    }
}
