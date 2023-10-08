using System.Collections.Generic;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Models;

namespace BoyumFoosballStats.Services.Interface;

public interface IMatchAnalysisService
{
    Dictionary<string, PlayerMatchStats> GetPlayerMatchStats(List<Match> matches);
    Dictionary<string, double> GetTableSideWinRates(List<Match> matches);
}