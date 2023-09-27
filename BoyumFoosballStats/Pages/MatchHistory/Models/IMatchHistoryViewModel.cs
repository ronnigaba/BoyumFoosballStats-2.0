using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Models;
using BoyumFoosballStats.Shared.DbModels;
using MudBlazor;

namespace BoyumFoosballStats.Pages.MatchHistory.Models;

public interface IMatchHistoryViewModel : IViewModelBase
{ 
    List<Match> Matches { get; set; }
    Task InitializeAsync();
    Color GetGreyScoreColor(Match match);
    Color GetBlackScoreColor(Match match);
    bool ShouldHidePager { get; }
    Task DeleteMatch(Match match);
}