using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Components.Charts.Models;
using BoyumFoosballStats.Models;
using BoyumFoosballStats.Shared.DbModels;
using MudBlazor;

namespace BoyumFoosballStats.Pages.PlayerDashboard.Models;

public interface IPlayerDashboardViewModel : IViewModelBase
{
    Task InitializeAsync();
    string? PlayerId { get; set; }
    Player? SelectedPlayer { get; set; }
    public List<Player> Players { get; }
    List<ChartDataItem> WinRateByWeekChartData { get; }
    List<ChartDataItem> MatchesWeekChartData { get; }
    List<ChartDataItem> WinRateByDayChartData { get; }
    int RankingsColumnLg { get; }
    int RankingsColumnXs { get; }
    void HandlePlayerClicked(Player player);
}