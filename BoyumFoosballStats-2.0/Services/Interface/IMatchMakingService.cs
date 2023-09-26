using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;

namespace BoyumFoosballStats_2._0.Services;

public interface IMatchMakingService
{
    Task<Match> FindFairestMatch(IEnumerable<Player> players);
}