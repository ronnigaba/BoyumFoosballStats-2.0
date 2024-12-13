using Moserware.Skills;
using Newtonsoft.Json;

namespace BoyumFoosballStats.Shared.Models;

public class TrueSkillRating : Rating
{
    public TrueSkillRating() : base(GameInfo.DefaultGameInfo.InitialMean,
        GameInfo.DefaultGameInfo.InitialStandardDeviation)
    {
    }

    public TrueSkillRating(Rating rating) : base(rating.Mean, rating.StandardDeviation, rating.ConservativeRating)
    {
    }

    [JsonConstructor]
    public TrueSkillRating(double mean, double standardDeviation, double conservativeRating) : base(mean,
        standardDeviation, conservativeRating)
    {
    }
}

public class TrueSkillRatings
{
    public TrueSkillRating? Overall { get; set; }
    public TrueSkillRating? Defender { get; set; }
    public TrueSkillRating? Attacker { get; set; }
}