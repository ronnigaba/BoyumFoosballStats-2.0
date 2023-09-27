using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Shared.DbModels;

namespace BoyumFoosballStats.Services.Interface;

public interface IMatchMakingService
{
    Task<Match> FindFairestMatch(IEnumerable<Player> players);
}