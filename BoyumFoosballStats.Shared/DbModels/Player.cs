using BoyumFoosballStats.Shared.Extensions;
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

    [JsonProperty("TrueSkillRatingAttacker")]
    public TrueSkillRating? TrueSkillRatingAttacker { get; set; }

    [JsonProperty("TrueSkillRatingDefender")]
    public TrueSkillRating? TrueSkillRatingDefender { get; set; }

    [JsonProperty("SeasonalTrueSkill")]
    public Dictionary<string, TrueSkillRatings?> SeasonalTrueSkill { get; set; } = new();

    [JsonIgnore]
    public TrueSkillRatings? CurrentSeasonRating
    {
        get
        {
            string seasonKey = DateTime.Now.GetSeasonKey();
            if (!SeasonalTrueSkill.ContainsKey(seasonKey))
            {
                SeasonalTrueSkill.Add(seasonKey, new TrueSkillRatings());
            }
            return SeasonalTrueSkill[seasonKey];
        }
        set
        {
            string seasonKey = DateTime.Now.Year + DateTime.Now.GetQuarter();
            SeasonalTrueSkill[seasonKey] = value;
        }
    }

    [JsonProperty("MatchesPlayed")] public int? MatchesPlayed { get; set; }

    [JsonProperty("MatchesPlayedAttacker")]
    public int? MatchesPlayedAttacker { get; set; }

    [JsonProperty("MatchesPlayedDefender")]
    public int? MatchesPlayedDefender { get; set; }

    [JsonProperty("LegacyPlayerId")] public int? LegacyPlayerId { get; set; }

    public override string ToString()
    {
        return Name ?? "";
    }
}