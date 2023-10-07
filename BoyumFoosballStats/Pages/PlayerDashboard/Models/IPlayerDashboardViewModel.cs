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
    List<WeekChartDataItem> WeekChartData { get; }
    public List<Player> Players { get; }
    int RankingsColumnLg { get; }
    int RankingsColumnXs { get; }
    void HandlePlayerClicked(Player player);
    void HandleClosePlayerStats();
}