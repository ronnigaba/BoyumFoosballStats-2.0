using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using MudBlazor;

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
        WeekChartData = new List<WeekChartDataItem>();
    }

    public string? PlayerId { get; set; }
    public List<WeekChartDataItem> WeekChartData { get; private set; }

    public List<Player> Players { get; private set; }

    public double GetWinRate(Player player)
    {
        var matchesWon = Matches.Count(m =>
            (m.BlackAttackerPlayer?.Id == player.Id && m.ScoreBlack > m.ScoreGrey) ||
            (m.BlackDefenderPlayer?.Id == player.Id && m.ScoreBlack > m.ScoreGrey) ||
            (m.GreyAttackerPlayer?.Id == player.Id && m.ScoreGrey > m.ScoreBlack) ||
            (m.GreyDefenderPlayer?.Id == player.Id && m.ScoreGrey > m.ScoreBlack));

        return (double)matchesWon / Matches.Count;
    }

    public string GetWinRateToString(Player player)
    {
        return $"{GetWinRate(player) * 100:0.##}%";
    }

    public async Task InitializeAsync()
    {
        Matches = (await _matchCrudService.GetAllAsync()).ToList();
        var players = (await _playerCrudService.GetAllAsync()).ToList();
        Players = players
            .OrderByDescending(x => x.Active)
            .ThenBy(x => x.TrueSkillRating?.StandardDeviation > 3)
            .ThenByDescending(x => x.TrueSkillRating?.Mean).ToList();
        Console.WriteLine($"Count: {Players.Count}");
        DisplayWinRateChart();
    }

    private void DisplayWinRateChart()
    {
        var last5WeeksData = GetPlayerWinRateForLast5Weeks();
        WeekChartData = last5WeeksData.Select(k => new WeekChartDataItem
        {
            Date = k.Key,
            WinRate = k.Value
        }).ToList();
    }
    
    private Dictionary<string, double> GetPlayerWinRateForLast5Weeks()
    {
        var endDate = DateTime.Today;
        var startDate = endDate.AddDays(-35); // Start of the 5th week from the current date

        var relevantMatches = Matches
            .Where(m => m.MatchDate >= startDate && m.MatchDate <= endDate)
            .Where(m =>
                m.BlackAttackerPlayer?.Id == PlayerId ||
                m.BlackDefenderPlayer?.Id == PlayerId ||
                m.GreyAttackerPlayer?.Id == PlayerId ||
                m.GreyDefenderPlayer?.Id == PlayerId).ToList();

        var winRateByWeek = new Dictionary<string, double>();

        var calendar = CultureInfo.CurrentCulture.Calendar;

        for (var i = 0; i < 5; i++)
        {
            var weekStartDate = startDate.AddDays(i * 7);
            var weekEndDate = weekStartDate.AddDays(6);

            var matchesThisWeek = relevantMatches.Where(m => m.MatchDate >= weekStartDate && m.MatchDate <= weekEndDate)
                .ToList();
            if (!matchesThisWeek.Any())
                continue;

            var matchesWon = matchesThisWeek.Count(m =>
                (m.BlackAttackerPlayer?.Id == PlayerId && m.ScoreBlack > m.ScoreGrey) ||
                (m.BlackDefenderPlayer?.Id == PlayerId && m.ScoreBlack > m.ScoreGrey) ||
                (m.GreyAttackerPlayer?.Id == PlayerId && m.ScoreGrey > m.ScoreBlack) ||
                (m.GreyDefenderPlayer?.Id == PlayerId && m.ScoreGrey > m.ScoreBlack));

            var winRate = (double)matchesWon / matchesThisWeek.Count;

            var weekNumber = calendar.GetWeekOfYear(weekStartDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            winRateByWeek[$"{weekNumber}"] = winRate; // Using week number as the key
        }

        return winRateByWeek;
    }

    public string FormatAsPercentage(object value)
    {
        // Convert the object value to double and format it as a percentage.
        return $"{Convert.ToDouble(value) * 100:0.##}%";
    }
}