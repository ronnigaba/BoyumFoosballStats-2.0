using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;
using BoyumFoosballStats_2.Components.TeamCard.ViewModel;
using BoyumFoosballStats.Models;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public interface IScoreCollectionViewModel : IViewModelBase
{
    public bool DrawerOpen { get; set; }
    bool ShowInactivePlayers { get; set; }
    bool AutoBalanceMatches { get; set; }
    bool AutoSwapPlayers { get; set; }
    IEnumerable<Player>? AvailablePlayers { get; set; }
    IEnumerable<Player>? SelectedPlayers { get; set; }
    TeamInfo GreyTeam { get; set; }
    TeamInfo BlackTeam { get; set; }
    void ToggleDrawer();
    Task LoadPlayers();
    string? PlayerToString(Player player);
    Task ShowActiveCheckedChanged(bool arg);
    Task SaveMatch();
    void HandleSelectedPlayersChanged(IEnumerable<Player> selectedPlayers);
    Task AutoBalance();
}