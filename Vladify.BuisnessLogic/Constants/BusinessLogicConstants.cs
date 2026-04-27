namespace Vladify.BuisnessLogic.Constants;

public static class BusinessLogicConstants
{
    public const int MaxAmountOfParallelThreadsForEmailNotification = 5;

    public const int NotificationBatchSize = 100;

    public const int ChunkSize = 20;

    public const string SongCreatedMessageTemplate =
        @"<p>{0} has posted {1} in his album {2}!</p>
            <p>Don't forget to check it up</p>";
}
