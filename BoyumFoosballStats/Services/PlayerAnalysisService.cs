﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Extensions;

namespace BoyumFoosballStats.Services;

public class PlayerAnalysisService : IPlayerAnalysisService
{
    
    public Dictionary<DateTime, double> GetPlayerWinRateForLast5Weeks(IEnumerable<Match> matches, string playerId)
    {
        var last5WeeksMatches = GetLastWeeksGroupedMatches(matches, playerId);

        var winRateByWeek = last5WeeksMatches
            .ToDictionary(
                g => g.First().MatchDate,
                g => GetWinRate(g.ToList(), playerId)
            );
        return winRateByWeek;
    }

    public Dictionary<DateTime, double> GetPlayerWinRateByWeekDay(IEnumerable<Match> matches, string playerId)
    {
        var relevantMatches = GetRelevantMatches(matches,playerId);

        // Grouping by day of the week and calculating win rate
        var winRateByWeekDay = relevantMatches
            .GroupBy(m => m.MatchDate.DayOfWeek.ToString())
            .OrderBy(g => (int)g.First().MatchDate.DayOfWeek)
            .ToDictionary(
                g => g.First().MatchDate,
                g => GetWinRate(g.ToList(), playerId)
            );

        return winRateByWeekDay;
    }

    public Dictionary<DateTime, int> GetMatchesPlayedForLast5Weeks(IEnumerable<Match> matches, string playerId)
    {
        var last5WeeksMatches = GetLastWeeksGroupedMatches(matches, playerId);

        var matchesByWeek = last5WeeksMatches
            .ToDictionary(
                g => g.First().MatchDate, 
                g => g.Count()
            );

        return matchesByWeek;
    }
    
    public Dictionary<DateTime, double> GetPlayerHighestTrueSkillForLast5Weeks(IEnumerable<Match> matches, string playerId)
    {
        var last5WeeksMatches = GetLastWeeksGroupedMatches(matches, playerId);

        var highestTrueSkillByWeek = last5WeeksMatches
            .ToDictionary(
                g => g.First().MatchDate, 
                g => GetHighestTrueSkill(g.ToList(), playerId)
            );

        return highestTrueSkillByWeek;
    }    
    
    public Dictionary<DateTime, double> GetPlayerHighestTrueSkillForLastWeeks(IEnumerable<Match> matches, string playerId, int lastWeeksNumber = 5)
    {
        var last5WeeksMatches = GetLastWeeksGroupedMatches(matches, playerId, lastWeeksNumber);

        var highestTrueSkillByWeek = last5WeeksMatches
            .ToDictionary(
                g => g.First().MatchDate, 
                g => GetHighestTrueSkill(g.ToList(), playerId)
            );

        return highestTrueSkillByWeek;
    }

    public Dictionary<DateTime, double> GetPlayerLowestTrueSkillForLast5Weeks(IEnumerable<Match> matches, string playerId)
    {
        var lowestTrueSkillByWeek = GetLastWeeksGroupedMatches(matches, playerId)
            .ToDictionary(
                g => g.First().MatchDate, 
                g => GetLowestTrueSkill(g.ToList(), playerId)
            );

        return lowestTrueSkillByWeek;
    }
    
    public Dictionary<DateTime, double> GetPlayerLowestTrueSkillForLastWeeks(IEnumerable<Match> matches, string playerId, int lastWeeksNumber = 5)
    {
        var lowestTrueSkillByWeek = GetLastWeeksGroupedMatches(matches, playerId, lastWeeksNumber)
            .ToDictionary(
                g => g.First().MatchDate, 
                g => GetLowestTrueSkill(g.ToList(), playerId)
            );

        return lowestTrueSkillByWeek;
    }

    private double GetHighestTrueSkill(List<Match> matches, string playerId)
    {
        return matches
            .Where(m => m.Players.Any(p => p.Id == playerId))
            .Max(m => m.Players.First(p => p.Id == playerId).TrueSkillRating.Mean); 
    }
    
    
    private IEnumerable<IGrouping<int, Match>> GetLastWeeksGroupedMatches(IEnumerable<Match> matches, string playerId, int lastWeeksNumber = 5)
    {
        return GetLastWeekMatches(matches, playerId, lastWeeksNumber)
            .GroupBy(m => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(m.MatchDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
            .OrderBy(g => g.Key)
            .TakeLast(lastWeeksNumber);
    }

    private double GetLowestTrueSkill(List<Match> matches, string playerId)
    {
        return matches
            .Where(m => m.Players.Any(p => p.Id == playerId))
            .Min(m => m.Players.First(p => p.Id == playerId).TrueSkillRating.Mean);
    }

    private List<Match> GetLastWeekMatches(IEnumerable<Match> matches, string playerId, int lastWeeksNumber = 5)
    {
        var relevantMatches = GetRelevantMatches(matches, playerId);
        
        var endDate = DateTime.Today.AddDays(1);
        var startDate = endDate.AddDays(-7 * lastWeeksNumber);

        var last5WeeksMatches = relevantMatches
            .Where(m => m.MatchDate >= startDate && m.MatchDate <= endDate)
            .ToList();
        return last5WeeksMatches;
    }

    private List<Match> GetRelevantMatches(IEnumerable<Match> matches, string playerId)
    {
        var relevantMatches = matches
            .Where(m => m.Players.Any(x => x?.Id == playerId))
            .ToList();
        return relevantMatches;
    }

    private double GetWinRate(List<Match> matches, string playerId)
    {
        var matchesWon = matches.Count(m =>
            (m.BlackAttackerPlayer?.Id == playerId && m.ScoreBlack > m.ScoreGrey) ||
            (m.BlackDefenderPlayer?.Id == playerId && m.ScoreBlack > m.ScoreGrey) ||
            (m.GreyAttackerPlayer?.Id == playerId && m.ScoreGrey > m.ScoreBlack) ||
            (m.GreyDefenderPlayer?.Id == playerId && m.ScoreGrey > m.ScoreBlack));

        return (double)matchesWon / matches.Count;
    }
}