namespace Vladify.IntegrationTests;

public static class TestConstants
{
    public const string Audience = "testAudience";

    public const string Issuer = "testIssuer";

    public const string TestSecretKey = "superMegaTestSecretKey";

    public const string BaseClaimNamespace = "https://vladify.com/";

    public const string CustomEmailClaimName = $"{BaseClaimNamespace}email";

    public const string UserNotificationSettingsCollectionName = "UserNotificationSettingsCollection";

    public const string GraphQlRoute = "/graphql";
}
