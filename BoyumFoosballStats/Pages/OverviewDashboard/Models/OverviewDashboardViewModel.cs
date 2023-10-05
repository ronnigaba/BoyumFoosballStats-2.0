using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Services.Extensions;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;

namespace BoyumFoosballStats.Pages.OverviewDashboard.Models;

public class OverviewDashboardViewModel : IOverviewDashboardViewModel
{
    public List<WinRateDataItem> WinRateMatchData { get; private set; } = new();
    public List<MatchesPlayedDataItem> MatchesPlayedMatchData { get; private set; } = new();
    public IEnumerable<string> FillColors { get; set; } = (new[] { "#594AE2", "#8D87DD" });

    private readonly IMatchCrudService _matchCrudService;
    private List<Match> _matches;

    public OverviewDashboardViewModel(IMatchCrudService matchCrudService)
    {
        _matchCrudService = matchCrudService;
        _matches = new List<Match>();
    }

    public async Task InitializeAsync()
    {
        _matches = (await _matchCrudService.GetAllAsync()).ToList();
        LoadPlayerMatchData();
    }

    private void LoadPlayerMatchData()
    {
        var winRates = _matches.GetPlayerWinRates();
        WinRateMatchData = winRates.Select(x => new WinRateDataItem { PlayerName = x.Key, WinRate = x.Value.WinRate })
            .OrderByDescending(x => x.WinRate).ToList();
        MatchesPlayedMatchData = winRates.Select(x => new MatchesPlayedDataItem
                { PlayerName = x.Key, MatchesPlayed = x.Value.MatchesPlayed })
            .OrderByDescending(x => x.MatchesPlayed).ToList();
    }
}

public class WinRateDataItem
{
    public string? PlayerName { get; set; }
    public double? WinRate { get; set; }
}

public class MatchesPlayedDataItem
{
    public string? PlayerName { get; set; }
    public int? MatchesPlayed { get; set; }
}