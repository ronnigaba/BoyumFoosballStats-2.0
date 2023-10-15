using System.Collections.Generic;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Models;

namespace BoyumFoosballStats.Services.Interface;

public interface IMatchAnalysisService
{
    void BustCache();
    Dictionary<string, PlayerMatchStats> GetPlayerMatchStats(List<Match> matches);
    Dictionary<string, double> GetTableSideWinRates(List<Match> matches);
    double GetTrueSkillChangeFromPreviousMatch(List<Match> matches, Player player, string? matchId);
}