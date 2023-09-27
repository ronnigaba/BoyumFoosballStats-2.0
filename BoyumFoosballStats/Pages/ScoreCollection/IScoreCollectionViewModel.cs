using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Components.TeamCard.Models;
using BoyumFoosballStats.Models;
using BoyumFoosballStats.Shared.DbModels;

namespace BoyumFoosballStats.Pages.ScoreCollection;

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