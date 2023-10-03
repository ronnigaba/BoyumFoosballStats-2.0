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
    private List<Match> Matches;

    public PlayerDashboardViewModel(IMatchCrudService matchCrudService)
    {
        _matchCrudService = matchCrudService;
        Matches = new List<Match>();
        WeekChartData = new List<WeekChartDataItem>();
    }

    public string? PlayerId { get; set; }
    public List<WeekChartDataItem> WeekChartData { get; private set;}
    
    public async Task InitializeAsync()
    {
        Matches = (await _matchCrudService.GetAllAsync()).ToList();
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
                (m.BlackAttackerPlayer?.Id == PlayerId) || 
                (m.BlackDefenderPlayer?.Id == PlayerId) || 
                (m.GreyAttackerPlayer?.Id == PlayerId) || 
                (m.GreyDefenderPlayer?.Id == PlayerId));

        var winRateByWeek = new Dictionary<string, double>();

        var calendar = CultureInfo.CurrentCulture.Calendar;

        for (int i = 0; i < 5; i++)
        {
            var weekStartDate = startDate.AddDays(i * 7);
            var weekEndDate = weekStartDate.AddDays(6);

            var matchesThisWeek = relevantMatches.Where(m => m.MatchDate >= weekStartDate && m.MatchDate <= weekEndDate).ToList();
            if (!matchesThisWeek.Any())
                continue;

            int matchesWon = matchesThisWeek.Count(m => 
                (m.BlackAttackerPlayer?.Id == PlayerId && m.ScoreBlack > m.ScoreGrey) || 
                (m.BlackDefenderPlayer?.Id == PlayerId && m.ScoreBlack > m.ScoreGrey) ||
                (m.GreyAttackerPlayer?.Id == PlayerId && m.ScoreGrey > m.ScoreBlack) ||
                (m.GreyDefenderPlayer?.Id == PlayerId && m.ScoreGrey > m.ScoreBlack));

            double winRate = (double)matchesWon / matchesThisWeek.Count;

            int weekNumber = calendar.GetWeekOfYear(weekStartDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
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

public class WeekChartDataItem
{
    public string Date { get; set; }
    public double WinRate { get; set; }
}