using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Cosmos.Services.Interfaces;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Extensions;

namespace BoyumFoosballStats.Pages.OverviewDashboard.Models;

public class OverviewDashboardViewModel : IOverviewDashboardViewModel
{
    public List<ChartDataItem<double>> WinRateChartData { get; private set; } = new();
    public List<ChartDataItem<int>> MatchesPlayedChartData { get; private set; } = new();
    public List<ChartDataItem<double>> TableSideWinRateChartData { get; private set; } = new();
    public IEnumerable<string> BarChartFillColors { get; } = (new[] { "#594AE2", "#58A2A3" });
    public IEnumerable<string> PieChartFillColor { get; } = new[] { "#131313", "#bfbfbf" };

    public List<string> AvailableSeasons { get; set; } = new();
    public string? SelectedSeason { get; set; }

    private readonly IMatchCrudService _matchCrudService;
    private readonly IMatchAnalysisService _matchAnalysisService;
    private List<Match> _allMatches;
    private List<Match> _matches;
    private List<IGrouping<string, Match>> _seasonGrouping;

    public OverviewDashboardViewModel(IMatchCrudService matchCrudService, IMatchAnalysisService matchAnalysisService)
    {
        _matchCrudService = matchCrudService;
        _matchAnalysisService = matchAnalysisService;
        _allMatches = new List<Match>();
        _matches = new List<Match>();
    }

    public async Task InitializeAsync()
    {
        _allMatches = (await _matchCrudService.GetAllAsync()).ToList();
        if (_allMatches.Any())
        {
            LoadSeasonalMatchData();
            LoadPlayerMatchData();
            LoadTableSideWinRateData();
        }
    }

    public void SeasonChanged(IEnumerable<string> arg)
    {
        SelectedSeason = arg.FirstOrDefault();
        _matches = _seasonGrouping.SingleOrDefault(x => x.Key == SelectedSeason)?.ToList() ?? _allMatches;
        LoadPlayerMatchData();
        LoadTableSideWinRateData();
    }

    private void LoadSeasonalMatchData()
    {
        _seasonGrouping = _allMatches.GroupBySeason().ToList();
        AvailableSeasons = _seasonGrouping.Select(x => x.Key).OrderDescending().ToList();
        SelectedSeason = AvailableSeasons.FirstOrDefault();
        _matches = _seasonGrouping.SingleOrDefault(x => x.Key == SelectedSeason)?.ToList() ?? _allMatches;
    }

    private void LoadTableSideWinRateData()
    {
        var winRates = _matchAnalysisService.GetTableSideWinRates(_matches);
        TableSideWinRateChartData =
            winRates.Select(x => new ChartDataItem<double> { Category = x.Key, Value = x.Value }).ToList();
    }

    private void LoadPlayerMatchData()
    {
        var winRates = _matchAnalysisService.GetPlayerMatchStats(_matches);
        WinRateChartData = winRates.Select(x => new ChartDataItem<double> { Category = x.Key, Value = x.Value.WinRate })
            .OrderByDescending(x => x.Value).ToList();
        MatchesPlayedChartData = winRates.Select(x => new ChartDataItem<int>
                { Category = x.Key, Value = x.Value.MatchesPlayed })
            .OrderByDescending(x => x.Value).ToList();
    }
}

public class ChartDataItem<T>
{
    public string? Category { get; set; }
    public T? Value { get; set; }
}