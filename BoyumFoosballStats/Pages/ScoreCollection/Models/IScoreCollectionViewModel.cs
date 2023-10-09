using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Components.TeamCard.Models;
using BoyumFoosballStats.Models;
using BoyumFoosballStats.Shared.DbModels;
using Microsoft.AspNetCore.Components.Web;

namespace BoyumFoosballStats.Pages.ScoreCollection.Models;

public interface IScoreCollectionViewModel : IViewModelBase
{
    IEnumerable<Player>? AvailablePlayers { get; set; }
    IEnumerable<Player> SelectedPlayers { get; set; }
    TeamInfo GreyTeam { get; set; }
    TeamInfo BlackTeam { get; set; }
    Task LoadPlayers();
    Task SaveMatch();
    Task HandleSelectedPlayersChanged(IEnumerable<Player> selectedPlayers);
    Task BalanceMatch();
    Task TeamInfoChanged(TeamInfo teamInfo);
    Task LoadSession();
    string GetInactivePlayerMenuText();
    string GetSessionMenuText();
    Task SessionMenuClicked(MouseEventArgs arg);
    Task InactivePlayerMenuClicked(MouseEventArgs arg);
}