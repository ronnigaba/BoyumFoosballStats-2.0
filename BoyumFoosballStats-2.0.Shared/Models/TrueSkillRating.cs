using Moserware.Numerics;

namespace BoyumFoosballStats_2._0.Shared.Models;

public class TrueSkillRating
{
    private const int ConservativeStandardDeviationMultiplier = 3;
    private readonly double _ConservativeStandardDeviationMultiplier;
    private readonly double _Mean;
    private readonly double _StandardDeviation;

    public TrueSkillRating(double mean, double standardDeviation)
        : this(mean, standardDeviation, 3.0)
    {
    }

    public TrueSkillRating(
        double mean,
        double standardDeviation,
        double conservativeStandardDeviationMultiplier)
    {
        this._Mean = mean;
        this._StandardDeviation = standardDeviation;
        this._ConservativeStandardDeviationMultiplier = conservativeStandardDeviationMultiplier;
    }

    public double Mean => this._Mean;

    public double StandardDeviation => this._StandardDeviation;

    public double ConservativeRating =>
        this._Mean - this._ConservativeStandardDeviationMultiplier * this._StandardDeviation;

    public static TrueSkillRating GetPartialUpdate(
        TrueSkillRating prior,
        TrueSkillRating fullPosterior,
        double updatePercentage)
    {
        GaussianDistribution gaussianDistribution1 = new GaussianDistribution(prior.Mean, prior.StandardDeviation);
        GaussianDistribution gaussianDistribution2 =
            new GaussianDistribution(fullPosterior.Mean, fullPosterior.StandardDeviation);
        double num1 = gaussianDistribution2.Precision - gaussianDistribution1.Precision;
        double num2 = updatePercentage * num1;
        double num3 = gaussianDistribution2.PrecisionMean - gaussianDistribution1.PrecisionMean;
        double num4 = updatePercentage * num3;
        GaussianDistribution gaussianDistribution3 =
            GaussianDistribution.FromPrecisionMean(gaussianDistribution1.PrecisionMean + num4,
                gaussianDistribution1.Precision + num2);
        return new TrueSkillRating(gaussianDistribution3.Mean, gaussianDistribution3.StandardDeviation,
            prior._ConservativeStandardDeviationMultiplier);
    }

    public override string ToString() =>
        string.Format("μ={0:0.0000}, σ={1:0.0000}", (object)this.Mean, (object)this.StandardDeviation);
}