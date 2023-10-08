using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Enums;
using BoyumFoosballStats.Pages.ScoreCollection;
using BoyumFoosballStats.Shared.DbModels;

namespace BoyumFoosballStats.Services.Interface;

public interface IMatchMakingService
{
    Task<Match> FindFairestMatch(List<Player> players, MatchMakingMethod method);
}