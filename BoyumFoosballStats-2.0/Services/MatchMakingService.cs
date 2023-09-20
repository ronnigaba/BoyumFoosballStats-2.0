using BoyumFoosballStats_2._0.Shared.FirestoreModels;

namespace BoyumFoosballStats_2._0.Services;

public class MatchMakingService : IMatchMakingService
{
    public MatchMakingService()
    {
    }

    public Match FindFairestMatch(List<Player> players)
    {
        var match = new Match();
        
        

        return match;
    }
}

public interface IMatchMakingService
{
    Match FindFairestMatch(List<Player> players);
}