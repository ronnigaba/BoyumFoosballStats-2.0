using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BoyumFoosballStats.Services.Extensions;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using MudBlazor.Extensions;

namespace BoyumFoosballStats.Services;

public class PlayerAnalysisService : IPlayerAnalysisService
{
    
    public Dictionary<string, double> GetPlayerWinRateForLast5Weeks(IEnumerable<Match> matches, string playerId)
    {
        var last5WeeksMatches = GetLast5WeekMatches(matches, playerId);

        // Group by week number, calculate win rate for each group, and take the last 5 weeks
        var winRateByWeek = last5WeeksMatches
            .GroupBy(m =>
                CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(m.MatchDate, CalendarWeekRule.FirstFullWeek,
                    DayOfWeek.Monday))
            .OrderBy(g => g.Key)
            .Take(5)
            .ToDictionary(
                g => g.Key.ToString("00"),
                g => GetWinRate(playerId, g.ToList())
            );

        return winRateByWeek;
    }

    public Dictionary<string, double> GetPlayerWinRateByWeekDay(IEnumerable<Match> matches, string playerId)
    {
        var relevantMatches = GetRelevantMatches(matches,playerId);

        // Grouping by day of the week and calculating win rate
        var winRateByWeekDay = relevantMatches
            .GroupBy(m => m.MatchDate.DayOfWeek.ToString())
            .OrderBy(g => (int)g.First().MatchDate.DayOfWeek) // Ensuring a consistent order
            .ToDictionary(
                g => g.Key,
                g => GetWinRate(playerId, g.ToList())
            );

        return winRateByWeekDay;
    }

    public Dictionary<DateTime, int> GetMatchesPlayedForLast5Weeks(IEnumerable<Match> matches, string playerId)
    {
        var last5WeeksMatches = GetLast5WeekMatches(matches, playerId);

        var matchesByWeek = last5WeeksMatches
            .GroupBy(m =>
                CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(m.MatchDate, CalendarWeekRule.FirstFullWeek,
                    DayOfWeek.Monday))
            .OrderBy(g => g.Key)
            .Take(5)
            .ToDictionary(
                g => g.First().MatchDate, 
                g => g.Count()
            );

        return matchesByWeek;
    }

    private List<Match> GetLast5WeekMatches(IEnumerable<Match> matches, string playerId)
    {
        var relevantMatches = GetRelevantMatches(matches, playerId);

        // Determine the last match date for the specified player
        var endDate = relevantMatches.MaxBy(m => m.MatchDate)?.MatchDate ?? DateTime.Today;
        var startDate = endDate.AddDays(-35);

        var last5WeeksMatches = relevantMatches
            .Where(m => m.MatchDate >= startDate && m.MatchDate <= endDate)
            .ToList();
        return last5WeeksMatches;
    }

    private List<Match> GetRelevantMatches(IEnumerable<Match> matches, string playerId)
    {
        var relevantMatches = matches
            .Where(m => m.WasPlayerInMatch(playerId))
            .ToList();
        return relevantMatches;
    }

    private double GetWinRate(string playerId, List<Match> matches)
    {
        var matchesWon = matches.Count(m =>
            (m.BlackAttackerPlayer?.Id == playerId && m.ScoreBlack > m.ScoreGrey) ||
            (m.BlackDefenderPlayer?.Id == playerId && m.ScoreBlack > m.ScoreGrey) ||
            (m.GreyAttackerPlayer?.Id == playerId && m.ScoreGrey > m.ScoreBlack) ||
            (m.GreyDefenderPlayer?.Id == playerId && m.ScoreGrey > m.ScoreBlack));

        return (double)matchesWon / matches.Count;
    }

    private string FormatWeekRange(DateTime dateInWeek)
    {
        // Calculate start and end dates of the week
        var weekStart = dateInWeek.StartOfWeek(DayOfWeek.Monday);
        var weekEnd = weekStart.AddDays(4); // Adding 4 days to get to Friday

        return $"{weekStart:MMM dd}-{weekEnd:dd}";
    }
}