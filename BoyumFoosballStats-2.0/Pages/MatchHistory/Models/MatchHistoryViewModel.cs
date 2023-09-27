using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared.DbModels;
using MudBlazor;

namespace BoyumFoosballStats_2.Pages.MatchHistory.Models;

public class MatchHistoryViewModel : IMatchHistoryViewModel
{
    private readonly IMatchCrudService _matchCrudService;
    private readonly IBrowserViewportService _browserViewportService;

    public MatchHistoryViewModel(IMatchCrudService matchCrudService, IBrowserViewportService browserViewportService)
    {
        _matchCrudService = matchCrudService;
        _browserViewportService = browserViewportService;
    }

    public List<Match> Matches { get; set; }

    public bool ShouldHidePager { get; private set; }
    public async Task DeleteMatch(Match match)
    {
        await _matchCrudService.DeleteAsync(match.Id!);
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