using System.ComponentModel.DataAnnotations;
using CosmosDb.Model;
using Newtonsoft.Json;

namespace BoyumFoosballStats.Shared.DbModels;

public class Match : CosmosDbBaseModel
{
    [JsonProperty("DsId")] public string? PartitionKey { get; set; } = "match";

    [JsonProperty("MatchDate")] public DateTime MatchDate { get; set; }

    [JsonProperty("LegacyMatchId")] public string? LegacyMatchId { get; set; }

    [JsonProperty("BlackAttackerPlayer")] public Player? BlackAttackerPlayer { get; set; }

    [JsonProperty("BlackDefenderPlayer")] public Player? BlackDefenderPlayer { get; set; }

    [JsonProperty("GrayAttackerPlayer")] public Player? GreyAttackerPlayer { get; set; }

    [JsonProperty("GrayDefenderPlayer")] public Player? GreyDefenderPlayer { get; set; }

    [JsonProperty("ScoreBlack")]
    [Range(0, 11, ErrorMessage = "Invalid score, valid values are 0-10")]
    public int ScoreBlack { get; set; }

    [JsonProperty("ScoreGray")]
    [Range(0, 11, ErrorMessage = "Invalid score, valid values are 0-10")]
    public int ScoreGrey { get; set; }

    public bool IsValid()
    {
        var players = Players;
        if (players.Any(x => x == null) || players.GroupBy(x => x).Any(y => y.Count() > 1))
        {
            return false;
        }

        return true;
    }

    public List<Player> Players => new()
        { BlackAttackerPlayer, BlackDefenderPlayer, GreyAttackerPlayer, GreyDefenderPlayer };
}