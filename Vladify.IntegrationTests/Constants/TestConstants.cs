namespace Vladify.IntegrationTests.Constants;

public static class TestConstants
{
    public const string Audience = "testAudience";

    public const string Issuer = "testIssuer";

    public static readonly string TestSecretKey = Environment.GetEnvironmentVariable("TEST_JWT_SECRETKEY")
            ?? throw new ArgumentException("Failed to get test jwt secret key");

    public const string BaseClaimNamespace = "https://vladify.com/";

    public const string CustomEmailClaimName = $"{BaseClaimNamespace}email";

    public const string UserNotificationSettingsCollectionName = "Notifications";

    public const string GraphQlRoute = "/graphql";

    public const string DbName = "TestDb";

}
