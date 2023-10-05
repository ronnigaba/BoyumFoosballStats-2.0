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
    List<WeekChartDataItem> WeekChartData { get; }
    string FormatAsPercentage(object value);
    public List<Player> Players { get; }
    double GetWinRate(Player player);
    string GetWinRateToString(Player player);
}