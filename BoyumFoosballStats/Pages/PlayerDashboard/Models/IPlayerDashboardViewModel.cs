using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Models;
using MudBlazor;

namespace BoyumFoosballStats.Pages.PlayerDashboard.Models;

public interface IPlayerDashboardViewModel : IViewModelBase
{
    Task InitializeAsync();
    string? PlayerId { get; set; }
    List<WeekChartDataItem> WeekChartData { get; }
    string FormatAsPercentage(object value);
}