using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BoyumFoosballStats.Pages.PlayerDashboard;

public partial class PlayerDashboard
{
    [Parameter] public string? PlayerId { get; set; }
    [Inject] public IMatchCrudService MatchCrudService { get; set; } = null!;

    public List<ChartSeries> Series = new();
    
    public string[] XAxisLabels;
    public int Index { get; set; } = -1;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var matches = await MatchCrudService.GetAllAsync();
        var last5WeeksData = GetPlayerWinRateForLast5Weeks(matches, PlayerId);

        var winRates = new List<double>();
        var weekLabels = new List<string>();

        foreach (var entry in last5WeeksData)
        {
            weekLabels.Add(entry.Key);
            winRates.Add(entry.Value * 100);
        }

        XAxisLabels = weekLabels.ToArray();
        Series.Add(new ChartSeries() { Name = "Win Rate by week (%)", Data = winRates.ToArray() });
    }
    
    public Dictionary<string, double> GetPlayerWinRateForLast5Weeks(IEnumerable<Match> matches, string playerId)
    {
        var endDate = DateTime.Today; 
        var startDate = endDate.AddDays(-35); // Start of the 5th week from the current date

        var relevantMatches = matches
            .Where(m => m.MatchDate >= startDate && m.MatchDate <= endDate)
            .Where(m => 
                (m.BlackAttackerPlayer?.Id == playerId) || 
                (m.BlackDefenderPlayer?.Id == playerId) || 
                (m.GreyAttackerPlayer?.Id == playerId) || 
                (m.GreyDefenderPlayer?.Id == playerId));

        var winRateByWeek = new Dictionary<string, double>();

        for (int i = 0; i < 5; i++)
        {
            var weekStartDate = startDate.AddDays(i * 7);
            var weekEndDate = weekStartDate.AddDays(6);

            var matchesThisWeek = relevantMatches.Where(m => m.MatchDate >= weekStartDate && m.MatchDate <= weekEndDate).ToList();
            if (!matchesThisWeek.Any())
                continue;

            int matchesWon = matchesThisWeek.Count(m => 
                (m.BlackAttackerPlayer?.Id == playerId && m.ScoreBlack > m.ScoreGrey) || 
                (m.BlackDefenderPlayer?.Id == playerId && m.ScoreBlack > m.ScoreGrey) ||
                (m.GreyAttackerPlayer?.Id == playerId && m.ScoreGrey > m.ScoreBlack) ||
                (m.GreyDefenderPlayer?.Id == playerId && m.ScoreGrey > m.ScoreBlack));

            double winRate = (double)matchesWon / matchesThisWeek.Count;

            winRateByWeek[weekStartDate.ToString("MMM dd")] = winRate; // Using "MMM dd" as an example format for the week's starting date
        }

        return winRateByWeek;
    }

}