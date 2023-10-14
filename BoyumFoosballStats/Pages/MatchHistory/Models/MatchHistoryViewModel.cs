using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BoyumFoosballStats.Pages.MatchHistory.Models;

public class MatchHistoryViewModel : IMatchHistoryViewModel
{
    private readonly IMatchCrudService _matchCrudService;
    private readonly IBrowserViewportService _browserViewportService;
    private readonly NavigationManager _navigationManager;

    public MatchHistoryViewModel(IMatchCrudService matchCrudService, IBrowserViewportService browserViewportService,
        NavigationManager navigationManager)
    {
        _matchCrudService = matchCrudService;
        _browserViewportService = browserViewportService;
        _navigationManager = navigationManager;
    }

    public List<Match> Matches { get; set; }

    public bool ShouldHidePager { get; private set; }

    public async Task DeleteMatch(Match match)
    {
        await _matchCrudService.DeleteAsync(match.Id!);
        Matches.Remove(match);
    }

    public void HandlePlayerClicked(Player player)
    {
        _navigationManager.NavigateTo("/PlayerDashboard/" + player.Id);
    }

    public string GetTrueSkillChange(Player player, string? matchId)
    {
        var trueSkillChange = "-";
        if (Matches.Any())
        {
            var matchIndex = Matches.FindIndex(x => x.Id == matchId);
            for (int i = matchIndex + 1; i <= Matches.Count; i++)
            {
                var match = Matches[i];
                if (!match.Players.Any(x => x.Id == player.Id))
                {
                    continue;
                }

                var prevMatchPlayer = match.Players.Single(x => x.Id == player.Id);

                trueSkillChange =
                    ((player.TrueSkillRating!.Mean - prevMatchPlayer!.TrueSkillRating!.Mean) * 100).ToString("+#;-#;0");
                break;
            }
        }

        return trueSkillChange;
    }

    public async Task InitializeAsync()
    {
        Matches = (await _matchCrudService.GetAllAsync()).Reverse().ToList();
        var windowSize = await _browserViewportService.GetCurrentBrowserWindowSizeAsync();
        if (windowSize.Width <= 600)
        {
            ShouldHidePager = true;
        }
    }

    public Color GetGreyScoreColor(Match match)
    {
        return match.ScoreGrey > match.ScoreBlack ? Color.Success : Color.Error;
    }

    public Color GetBlackScoreColor(Match match)
    {
        return match.ScoreBlack > match.ScoreGrey ? Color.Success : Color.Error;
    }
}