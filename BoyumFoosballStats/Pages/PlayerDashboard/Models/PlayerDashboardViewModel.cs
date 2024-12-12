using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Components.Charts.Models;
using BoyumFoosballStats.Enums;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using MudBlazor.Utilities;

namespace BoyumFoosballStats.Pages.PlayerDashboard.Models;

public class PlayerDashboardViewModel : IPlayerDashboardViewModel
{
    private readonly IMatchCrudService _matchCrudService;
    private readonly IPlayerCrudService _playerCrudService;
    private readonly IPlayerAnalysisService _playerAnalysisService;
    private List<Match> Matches;

    public PlayerDashboardViewModel(IMatchCrudService matchCrudService, IPlayerCrudService playerCrudService,
        IPlayerAnalysisService playerAnalysisService)
    {
        _matchCrudService = matchCrudService;
        _playerCrudService = playerCrudService;
        _playerAnalysisService = playerAnalysisService;
        Players = new List<Player>();
        Matches = new List<Match>();
        WinRateByWeekChartData = new List<ChartDataItem>();
    }

    public string? PlayerId { get; set; }
    public Player? SelectedPlayer { get; set; }
    public List<ChartDataItem> WinRateByWeekChartData { get; private set; }
    public List<ChartDataItem> MatchesWeekChartData { get; private set; }
    public List<ChartDataItem> WinRateByDayChartData { get; private set; }
    public List<ChartDataItem> HighestTrueSkillByWeekChartData { get; private set; }
    public List<ChartDataItem> LowestTrueSkillByWeekChartData { get; private set; }
    public int RankingsColumnLg => SelectedPlayer is null ? 12 : 4;
    public int RankingsColumnXs => 12;
    public int ChartsColumnLg => SelectedPlayer is null ? 0 : 8;
    public int ChartsColumnXs => SelectedPlayer is null ? 0 : 12;

    public string ChartsGridItemClasses => new CssBuilder("grid-item-transition overflow-hidden")
        .AddClass("pa-0 grid-item-hidden", SelectedPlayer is null)
        .Build();

    public string RankingsGridItemClasses => new CssBuilder("grid-item-transition flex-1 mud-width-full")
        .Build();

    public List<Player> Players { get; private set; }

    public async Task InitializeAsync()
    {
        Matches = (await _matchCrudService.GetAllAsync()).ToList();
        Players = (await _playerCrudService.GetAllAsync()).ToList();
        SortPlayers(PlayerPosition.Overall);
        if (!string.IsNullOrEmpty(PlayerId))
        {
            SelectedPlayer = Players.First(x => x.Id == PlayerId);
            DisplayCharts(PlayerId);
        }
    }

    public void SortPlayers(PlayerPosition position)
    {
        if (Players?.Any() != null)
        {
            switch (position)
            {
                case PlayerPosition.Overall:
                    Players = Players
                        .OrderByDescending(x => x.Active)
                        .ThenBy(x => x.TrueSkillRating?.StandardDeviation > 3)
                        .ThenByDescending(x => x.TrueSkillRating?.Mean).ToList();
                    break;
                case PlayerPosition.Attacker:
                    Players = Players
                        .OrderByDescending(x => x.Active)
                        .ThenBy(x => x.TrueSkillRatingAttacker?.StandardDeviation > 3)
                        .ThenByDescending(x => x.TrueSkillRatingAttacker?.Mean).ToList();
                    break;
                case PlayerPosition.Defender:
                    Players = Players
                        .OrderByDescending(x => x.Active)
                        .ThenBy(x => x.TrueSkillRatingDefender?.StandardDeviation > 3)
                        .ThenByDescending(x => x.TrueSkillRatingDefender?.Mean).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }
    }

    public void HandlePlayerClicked(Player player)
    {
        SelectedPlayer = player;
        DisplayCharts(player.Id);
    }

    private void DisplayCharts(string? playerId)
    {
        var winRateData = _playerAnalysisService.GetPlayerWinRateForLast5Weeks(Matches, playerId!);
        WinRateByWeekChartData = winRateData.Select(k => new ChartDataItem
        {
            XData = k.Key,
            YData = k.Value
        }).ToList();

        var matchesPlayedData = _playerAnalysisService.GetMatchesPlayedForLast5Weeks(Matches, playerId!);
        MatchesWeekChartData = matchesPlayedData.Select(k => new ChartDataItem
        {
            XData = k.Key,
            YData = k.Value
        }).ToList();

        var winRateByDayData = _playerAnalysisService.GetPlayerWinRateByWeekDay(Matches, playerId!);
        WinRateByDayChartData = winRateByDayData.Select(k => new ChartDataItem
        {
            XData = k.Key,
            YData = k.Value
        }).ToList();

        var highestTrueSkillByWeekData =
            _playerAnalysisService.GetPlayerHighestTrueSkillForLastWeeks(Matches, playerId!, 10);
        HighestTrueSkillByWeekChartData = highestTrueSkillByWeekData.Select(k => new ChartDataItem
        {
            XData = k.Key,
            YData = k.Value
        }).ToList();

        var lowestTrueSkillByWeekData =
            _playerAnalysisService.GetPlayerLowestTrueSkillForLastWeeks(Matches, playerId!, 10);
        LowestTrueSkillByWeekChartData = lowestTrueSkillByWeekData.Select(k => new ChartDataItem
        {
            XData = k.Key,
            YData = k.Value
        }).ToList();
    }
}