using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Components.Charts.Models;
using BoyumFoosballStats.Enums;
using BoyumFoosballStats.Models;
using BoyumFoosballStats.Shared.DbModels;

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
    List<ChartDataItem> HighestTrueSkillByWeekChartData { get; }
    List<ChartDataItem> LowestTrueSkillByWeekChartData { get; }
    int RankingsColumnLg { get; }
    int RankingsColumnXs { get; }
    int ChartsColumnLg { get; }
    int ChartsColumnXs { get; }
    string ChartsGridItemClasses { get;}
    string RankingsGridItemClasses { get;}
    void HandlePlayerClicked(Player player);
    void SortPlayers(PlayerPosition position);
}