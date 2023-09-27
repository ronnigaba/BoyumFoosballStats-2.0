using Moserware.Skills;
using Newtonsoft.Json;

namespace BoyumFoosballStats_2._0.Shared.Models;

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