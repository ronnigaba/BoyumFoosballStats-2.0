using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Models;

namespace BoyumFoosballStats.Pages.OverviewDashboard.Models;

public interface IOverviewDashboardViewModel : IViewModelBase
{
    List<WinRateDataItem>? WinRateMatchData { get; }
    List<MatchesPlayedDataItem>? MatchesPlayedMatchData { get; }
    IEnumerable<string> FillColors { get; set; }
    Task InitializeAsync();
}