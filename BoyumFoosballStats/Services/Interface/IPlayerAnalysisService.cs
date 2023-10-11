﻿using System;
using System.Collections.Generic;
using BoyumFoosballStats.Shared.DbModels;

namespace BoyumFoosballStats.Services.Interface;

public interface IPlayerAnalysisService
{
    Dictionary<DateTime, double> GetPlayerWinRateForLast5Weeks(IEnumerable<Match> matches, string playerId);
    Dictionary<DateTime, double> GetPlayerWinRateByWeekDay(IEnumerable<Match> matches, string playerId);
    Dictionary<DateTime, int> GetMatchesPlayedForLast5Weeks(IEnumerable<Match> matches, string playerId);
    Dictionary<DateTime, double> GetPlayerHighestTrueSkillForLast5Weeks(IEnumerable<Match> matches, string playerId);
    Dictionary<DateTime, double> GetPlayerLowestTrueSkillForLast5Weeks(IEnumerable<Match> matches, string playerId);
}