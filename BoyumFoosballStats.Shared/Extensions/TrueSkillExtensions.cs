namespace BoyumFoosballStats.Shared.Extensions;

public static class TrueSkillExtensions
{
    public static string ToTrueSkillString(this double trueSkillMean)
    {
        return ((int)(trueSkillMean * 100)).ToString();
    }
}