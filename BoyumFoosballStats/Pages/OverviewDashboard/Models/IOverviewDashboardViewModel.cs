using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Models;

namespace BoyumFoosballStats.Pages.OverviewDashboard.Models;

public interface IOverviewDashboardViewModel : IViewModelBase
{
    List<ChartDataItem<double>> WinRateChartData { get; }
    List<ChartDataItem<int>> MatchesPlayedChartData { get; }
    IEnumerable<string> BarChartFillColors { get; }
    IEnumerable<string> PieChartFillColor { get; }
    List<ChartDataItem<double>> TableSideWinRateChartData { get; }
    List<string> AvailableSeasons { get; set; }
    string SelectedSeason { get; set; }
    Task InitializeAsync();
    void SeasonChanged(IEnumerable<string> value);
}