using BoyumFoosballStats.Shared.Enums;
using CosmosDb.Model;
using Newtonsoft.Json;

namespace BoyumFoosballStats.Shared.DbModels;

public class Session : CosmosDbBaseModel
{
    [JsonProperty("DsId")] public string? PartitionKey { get; set; } = "session";
    [JsonProperty("State")] public SessionState State { get; set; } = SessionState.Active;
    [JsonProperty("Matches")] public List<Match> Matches { get; set; } = new();
    [JsonProperty("SelectedPlayerIds")] public List<string?> SelectedPlayers { get; set; } = new();
    [JsonProperty("BlackAttackerId")] public string? BlackAttackerId { get; set; }
    [JsonProperty("BlackDefenderId")] public string? BlackDefenderId { get; set; }
    [JsonProperty("GreyAttackerId")] public string? GreyAttackerId { get; set; }
    [JsonProperty("GreyDefenderId")] public string? GreyDefenderId { get; set; }
    [JsonProperty("StartDate")] public DateTime StartDate { get; set; } = DateTime.Now;
    [JsonProperty("EndDate")] public DateTime? EndDate { get; set; }
    [JsonProperty("ShowInactivePlayers")]public bool ShowInactivePlayers { get; set; }
}