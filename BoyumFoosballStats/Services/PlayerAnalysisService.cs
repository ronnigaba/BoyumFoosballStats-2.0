using System;
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
        var last5WeeksMatches = GetLast5WeekMatches(matches, playerId);

        // Group by week number, calculate win rate for each group, and take the last 5 weeks

        var winRateByWeek = last5WeeksMatches
            .GroupBy(m =>
                CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(m.MatchDate, CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday))
            .OrderBy(g => g.Key)
            .TakeLast(5)
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
        var last5WeeksMatches = GetLast5WeekMatches(matches, playerId);

        var matchesByWeek = last5WeeksMatches
            .GroupBy(m =>
                CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(m.MatchDate, CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday))
            .OrderBy(g => g.Key)
            .TakeLast(5)
            .ToDictionary(
                g => g.First().MatchDate, 
                g => g.Count()
            );

        return matchesByWeek;
    }

    private List<Match> GetLast5WeekMatches(IEnumerable<Match> matches, string playerId)
    {
        var relevantMatches = GetRelevantMatches(matches, playerId);
        
        var endDate = DateTime.Today.AddDays(1);
        var startDate = endDate.AddDays(-35);

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