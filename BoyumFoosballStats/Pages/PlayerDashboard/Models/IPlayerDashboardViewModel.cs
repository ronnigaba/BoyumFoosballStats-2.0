using System.Collections.Generic;
using System.Threading.Tasks;
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
    string FormatAsPercentage(object value);
    public List<Player> Players { get; }
    string GetWinRateToString(Player player);
    double MaxTrueSkill { get;  }
    double MinTrueSkill { get;  }
    double MaxGames { get; }
    double MinGames { get;  }
    int RankingsColumnLg { get; }
    int RankingsColumnXs { get; }
    int GetRankingNumber(Player player);
    Task HandlePlayerClicked(TableRowClickEventArgs<Player> args);
    void HandleClosePlayerStats();
}