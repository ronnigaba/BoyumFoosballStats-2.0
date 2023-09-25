namespace BoyumFoosballStats_2._0.Shared.Models;

public class TrueSkillGameInfo
{
    private const double DefaultBeta = 4.166666666666667;
    private const double DefaultDrawProbability = 0.1;
    private const double DefaultDynamicsFactor = 0.08333333333333333;
    private const double DefaultInitialMean = 25.0;
    private const double DefaultInitialStandardDeviation = 8.333333333333334;

    public TrueSkillGameInfo(
        double initialMean,
        double initialStandardDeviation,
        double beta,
        double dynamicFactor,
        double drawProbability)
    {
        this.InitialMean = initialMean;
        this.InitialStandardDeviation = initialStandardDeviation;
        this.Beta = beta;
        this.DynamicsFactor = dynamicFactor;
        this.DrawProbability = drawProbability;
    }

    public double InitialMean { get; set; }

    public double InitialStandardDeviation { get; set; }

    public double Beta { get; set; }

    public double DynamicsFactor { get; set; }

    public double DrawProbability { get; set; }

    public TrueSkillRating DefaultRating => new TrueSkillRating(this.InitialMean, this.InitialStandardDeviation);

    public static TrueSkillGameInfo DefaultGameInfo =>
        new TrueSkillGameInfo(25.0, 25.0 / 3.0, 25.0 / 6.0, 1.0 / 12.0, 0.1);
}