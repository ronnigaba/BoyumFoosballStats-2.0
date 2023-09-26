using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;
using BoyumFoosballStats_2.Components.TeamCard.ViewModel;
using BoyumFoosballStats.Models;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public interface IScoreCollectionViewModel : IViewModelBase
{
    IEnumerable<Player>? AvailablePlayers { get; set; }
    IEnumerable<Player>? SelectedPlayers { get; set; }
    TeamInfo GreyTeam { get; set; }
    TeamInfo BlackTeam { get; set; }
    Task LoadPlayers();
    string? PlayerToString(Player player);
    Task SaveMatch();
    void HandleSelectedPlayersChanged(IEnumerable<Player> selectedPlayers);
    Task BalanceMatch();
}