using BoyumFoosballStats.Shared.Models;
using CosmosDb.Model;
using Newtonsoft.Json;

namespace BoyumFoosballStats.Shared.DbModels;

public class Player : CosmosDbBaseModel
{
    [JsonProperty("DsId")] public string? PartitionKey { get; set; } = "player";
    
    [JsonProperty("Name")] public string? Name { get; set; }

    [JsonProperty("Active")] public bool Active { get; set; }

    [JsonProperty("TrueSkillRating")] public TrueSkillRating? TrueSkillRating { get; set; }

    [JsonProperty("MatchesPlayed")] public int? MatchesPlayed { get; set; }

    [JsonProperty("LegacyPlayerId")] public int? LegacyPlayerId { get; set; }
}