using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;
using BoyumFoosballStats.Models;
using MudBlazor;

namespace BoyumFoosballStats_2.Pages.MatchHistory.Models;

public interface IMatchHistoryViewModel : IViewModelBase
{ 
    List<Match> Matches { get; set; }
    Task InitializeAsync();
    Color GetGreyScoreColor(Match match);
    Color GetBlackScoreColor(Match match);
    bool ShouldHidePager { get; }
    Task DeleteMatch(Match match);
}