using BoyumFoosballStats.Shared.Enums;
using CosmosDb.Model;
using Newtonsoft.Json;

namespace BoyumFoosballStats.Shared.DbModels;

public class Session : CosmosDbBaseModel
{
    [JsonProperty("DsId")] public string? PartitionKey { get; set; } = "session";
    [JsonProperty("State")] public SessionState State { get; set; } = SessionState.Active;

    [JsonProperty("Matches")] public List<Match> Matches { get; set; } = new();
    [JsonProperty("SelectedPlayers")] public List<Player>? SelectedPlayers { get; set; } = new();
    [JsonProperty("BlackAttackerPlayer")] public Player? BlackAttackerPlayer { get; set; }
    [JsonProperty("BlackDefenderPlayer")] public Player? BlackDefenderPlayer { get; set; }
    [JsonProperty("GreyAttackerPlayer")] public Player? GreyAttackerPlayer { get; set; }
    [JsonProperty("GreyDefenderPlayer")] public Player? GreyDefenderPlayer { get; set; }
    [JsonProperty("StartDate")] public DateTime StartDate { get; set; } = DateTime.Now;
    [JsonProperty("EndDate")] public DateTime? EndDate { get; set; }
}