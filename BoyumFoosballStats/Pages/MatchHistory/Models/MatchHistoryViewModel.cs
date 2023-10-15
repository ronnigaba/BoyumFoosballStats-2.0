using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BoyumFoosballStats.Pages.MatchHistory.Models;

public class MatchHistoryViewModel : IMatchHistoryViewModel
{
    private readonly IMatchCrudService _matchCrudService;
    private readonly IBrowserViewportService _browserViewportService;
    private readonly NavigationManager _navigationManager;
    private readonly IMatchAnalysisService _matchAnalysisService;

    public MatchHistoryViewModel(IMatchCrudService matchCrudService, IBrowserViewportService browserViewportService,
        NavigationManager navigationManager, IMatchAnalysisService matchAnalysisService)
    {
        _matchCrudService = matchCrudService;
        _browserViewportService = browserViewportService;
        _navigationManager = navigationManager;
        _matchAnalysisService = matchAnalysisService;
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
        return _matchAnalysisService.GetTrueSkillChangeFromPreviousMatch(Matches, player, matchId).ToTrueSkillChangeString();
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