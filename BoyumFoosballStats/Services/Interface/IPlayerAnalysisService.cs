using System;
using System.Collections.Generic;
using BoyumFoosballStats.Shared.DbModels;

namespace BoyumFoosballStats.Services.Interface;

public interface IPlayerAnalysisService
{
    Dictionary<string, double> GetPlayerWinRateForLast5Weeks(IEnumerable<Match> matches, string playerId);
    Dictionary<string, double> GetPlayerWinRateByWeekDay(IEnumerable<Match> matches, string playerId);
    Dictionary<DateTime, int> GetMatchesPlayedForLast5Weeks(IEnumerable<Match> matches, string playerId);
}