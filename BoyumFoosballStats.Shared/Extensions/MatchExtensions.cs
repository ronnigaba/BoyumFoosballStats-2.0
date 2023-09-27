using BoyumFoosballStats.Shared.DbModels;

namespace BoyumFoosballStats.Shared.Extensions;

public static class MatchExtensions
{
    public static IEnumerable<IGrouping<string, Match>> GroupBySeason(this List<Match> matches)
    {
        return matches.GroupBy(x => x.MatchDate.ToString("yyyy/MM"));
    }
}