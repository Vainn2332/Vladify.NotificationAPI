namespace Vladify.IntegrationTests.Constants;

public static class TestConstants
{
    public const string Audience = "testAudience";

    public const string Issuer = "testIssuer";

    public const string TestSecretKey = "superMegaTestSecretKeyThatMustBeVeryLargeInOrderNotToThrowArgumentException";

    public const string BaseClaimNamespace = "https://vladify.com/";

    public const string CustomEmailClaimName = $"{BaseClaimNamespace}email";

    public const string UserNotificationSettingsCollectionName = "Notifications";

    public const string GraphQlRoute = "/graphql";
}
