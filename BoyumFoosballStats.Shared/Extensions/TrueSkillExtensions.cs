namespace BoyumFoosballStats.Shared.Extensions;

public static class TrueSkillExtensions
{
    public static string ToTrueSkillString(this double trueSkillMean)
    {
        return ((int)(trueSkillMean * 100)).ToString();
    }
    
    public static string ToTrueSkillChangeString(this double trueSkillChange)
    {
        return ((int)(trueSkillChange * 100)).ToString("+#;-#;0");
    }
}