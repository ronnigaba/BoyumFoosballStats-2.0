namespace BoyumFoosballStats.Shared;

public static class BoyumFoosballStatsConsts
{
    public static string ProjectName => "boyum-foosball-stats";
    public static string DefaultBucketName => $"{ProjectName}.appspot.com";
    public static string AiModelName => "MatchOutcomeModel.zip";
    public static string PlayerContainernName = "players";
    public static string MatchesContainerName = "matches";
    public static string PlayerPartitionKey = "player";
    public static string MatchesPartitionKey = "match";
}