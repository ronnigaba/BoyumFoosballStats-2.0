using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Components.Charts.Models;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using MudBlazor;
using MudBlazor.Extensions;

namespace BoyumFoosballStats.Pages.PlayerDashboard.Models;

public class PlayerDashboardViewModel : IPlayerDashboardViewModel
{
    private readonly IMatchCrudService _matchCrudService;
    private readonly IPlayerCrudService _playerCrudService;
    private List<Match> Matches;

    public PlayerDashboardViewModel(IMatchCrudService matchCrudService, IPlayerCrudService playerCrudService)
    {
        _matchCrudService = matchCrudService;
        _playerCrudService = playerCrudService;
        Players = new List<Player>();
        Matches = new List<Match>();
        WinRateByWeekChartData = new List<ChartDataItem>();
    }

    public string? PlayerId { get; set; }
    public Player? SelectedPlayer { get; set; }
    public List<ChartDataItem> WinRateByWeekChartData { get; private set; }
    public List<ChartDataItem> MatchesWeekChartData { get; private set; }
    public List<ChartDataItem> WinRateByDayChartData { get; private set; }
    public int RankingsColumnLg => SelectedPlayer is null ? 12 : 4;
    public int RankingsColumnXs => 12;
    public List<Player> Players { get; private set; }

    public async Task InitializeAsync()
    {
        Matches = (await _matchCrudService.GetAllAsync()).ToList();
        var players = (await _playerCrudService.GetAllAsync()).ToList();
        Players = players
            .OrderByDescending(x => x.Active)
            .ThenBy(x => x.TrueSkillRating?.StandardDeviation > 3)
            .ThenByDescending(x => x.TrueSkillRating?.Mean).ToList();
        
        if (!string.IsNullOrEmpty(PlayerId))
        {
            SelectedPlayer = Players.First(x => x.Id == PlayerId);
            DisplayCharts(PlayerId);
        }
    }

    public void HandlePlayerClicked(Player player)
    {
        SelectedPlayer = player;
        DisplayCharts(player.Id);
    }

    public string GetWinRateToString(Player player)
    {
        return $"{GetWinRate(player.Id!, Matches) * 100:0.##}%";
    }

    private void DisplayCharts(string? playerId)
    {
        var winRateData = GetPlayerWinRateForLast5Weeks(playerId!);
        WinRateByWeekChartData = winRateData.Select(k => new ChartDataItem
        {
            XData = k.Key,
            YData = k.Value
        }).ToList();
        
        var matchesPlayedData = GetMatchesPlayedForLast5Weeks(playerId!);
        MatchesWeekChartData = matchesPlayedData.Select(k => new ChartDataItem
        {
            XData = k.Key,
            YData = k.Value
        }).ToList();
        
        var winRateByDayData = GetPlayerWinRateByWeekDay(playerId!);
        WinRateByDayChartData = winRateByDayData.Select(k => new ChartDataItem
        {
            XData = k.Key,
            YData = k.Value
        }).ToList();
    }

    private Dictionary<string, double> GetPlayerWinRateForLast5Weeks(string playerId)
    {
        var last5WeeksMatches = GetLast5WeekMatches(playerId);

        // Group by week number, calculate win rate for each group, and take the last 5 weeks
        var winRateByWeek = last5WeeksMatches
            .GroupBy(m => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(m.MatchDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday))
            .OrderBy(g => g.Key)
            .Take(5)
            .ToDictionary(
                g => g.Key.ToString("00"), 
                g => GetWinRate(playerId, g.ToList())
            );

        return winRateByWeek;
    }

    private Dictionary<string, double> GetPlayerWinRateByWeekDay(string playerId)
    {
        var relevantMatches = GetRelevantMatches(playerId);

        // Grouping by day of the week and calculating win rate
        var winRateByWeekDay = relevantMatches
            .GroupBy(m => m.MatchDate.DayOfWeek.ToString())
            .OrderBy(g => (int)g.First().MatchDate.DayOfWeek)  // Ensuring a consistent order
            .ToDictionary(
                g => g.Key, 
                g => GetWinRate(playerId, g.ToList())
            );

        return winRateByWeekDay;
    }

    private List<Match> GetLast5WeekMatches(string playerId)
    {
        var relevantMatches = GetRelevantMatches(playerId);

        // Determine the last match date for the specified player
        var endDate = relevantMatches.MaxBy(m => m.MatchDate)?.MatchDate ?? DateTime.Today;
        var startDate = endDate.AddDays(-35);

        var last5WeeksMatches = relevantMatches
            .Where(m => m.MatchDate >= startDate && m.MatchDate <= endDate)
            .ToList();
        return last5WeeksMatches;
    }

    private List<Match> GetRelevantMatches(string playerId)
    {
        var relevantMatches = Matches
            .Where(m =>
                m.BlackAttackerPlayer?.Id == playerId ||
                m.BlackDefenderPlayer?.Id == playerId ||
                m.GreyAttackerPlayer?.Id == playerId ||
                m.GreyDefenderPlayer?.Id == playerId)
            .ToList();
        return relevantMatches;
    }

    private Dictionary<string, int> GetMatchesPlayedForLast5Weeks(string playerId)
    {
        var last5WeeksMatches = GetLast5WeekMatches(playerId);
        
        var matchesByWeek = last5WeeksMatches
            .GroupBy(m => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(m.MatchDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday))
            .OrderBy(g => g.Key)
            .Take(5)
            .ToDictionary(
                g => FormatWeekRange(g.First().MatchDate), 
                g => g.Count()
            );

        return matchesByWeek;
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