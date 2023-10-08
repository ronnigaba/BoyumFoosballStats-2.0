namespace BoyumFoosballStats.Shared;

public static class BoyumFoosballStatsConsts
{
    public static string ProjectName => "boyum-foosball-stats";
    public static string DefaultBucketName => $"{ProjectName}.appspot.com";
    public const string AiModelName = "MatchOutcomeModel.zip";
    public const string PlayerPartitionKey = "player";
    public const string PlayerContainerName = "players";
    public const string MatchesPartitionKey = "match";
    public const string MatchesContainerName = "matches";
    public const string SessionsPartitionKey = "session";
    public const string SessionsContainerName = "sessions";
    public const string SessionIdLocalStorageKey = "SessionId";

    public const string GreyTeamName = "Grey";
    public const string BlackTeamName = "Black";
    public const string DebugPrefix = "debug_";
}