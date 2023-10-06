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
    public Player? SelectedPlayer { get; set; }
    public List<WeekChartDataItem> WeekChartData { get; private set; }
    public double MaxTrueSkill { get; private set; }
    public double MinTrueSkill => 0;
    public double MaxGames { get; private set; }
    public double MinGames => 0;
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

        MaxTrueSkill = Players.First().TrueSkillRating!.Mean;

        MaxGames = (double) Players.MaxBy(x => x.MatchesPlayed)!.MatchesPlayed!;
        
        if (!string.IsNullOrEmpty(PlayerId))
        {
            DisplayWinRateChart(PlayerId);
        }
    }

    public int GetRankingNumber(Player player)
    {
        return Players.IndexOf(player) + 1;
    }

    public Task HandlePlayerClicked(TableRowClickEventArgs<Player> args)
    {
        SelectedPlayer = args.Item;
        DisplayWinRateChart(args.Item.Id);
        return Task.CompletedTask;
    }

    public void HandleClosePlayerStats()
    {
        SelectedPlayer = null;
    }

    public string GetWinRateToString(Player player)
    {
        return $"{GetWinRate(player.Id!, Matches) * 100:0.##}%";
    }

    public string FormatAsPercentage(object value)
    {
        // Convert the object value to double and format it as a percentage.
        return $"{Convert.ToDouble(value) * 100:0.##}%";
    }

    private void DisplayWinRateChart(string? playerId)
    {
        var last5WeeksData = GetPlayerWinRateForLast5Weeks(playerId);
        WeekChartData = last5WeeksData.Select(k => new WeekChartDataItem
        {
            Date = k.Key,
            WinRate = k.Value
        }).ToList();
    }

    private Dictionary<string, double> GetPlayerWinRateForLast5Weeks(string playerId)
    {
        var endDate = DateTime.Today;
        var startDate = endDate.AddDays(-35); // Start of the 5th week from the current date

        var relevantMatches = Matches
            .Where(m => m.MatchDate >= startDate && m.MatchDate <= endDate)
            .Where(m =>
                m.BlackAttackerPlayer?.Id == playerId ||
                m.BlackDefenderPlayer?.Id == playerId ||
                m.GreyAttackerPlayer?.Id == playerId ||
                m.GreyDefenderPlayer?.Id == playerId).ToList();

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

            var winRate = GetWinRate(playerId, matchesThisWeek);

            var weekNumber = calendar.GetWeekOfYear(weekStartDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            winRateByWeek[$"{weekNumber}"] = winRate; // Using week number as the key
        }

        return winRateByWeek;
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
}