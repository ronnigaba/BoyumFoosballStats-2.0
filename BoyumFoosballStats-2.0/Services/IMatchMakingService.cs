using BoyumFoosballStats_2._0.Shared.FirestoreModels;

namespace BoyumFoosballStats_2._0.Services;

public interface IMatchMakingService
{
    Task<Match> FindFairestMatch(List<Player> players);
}