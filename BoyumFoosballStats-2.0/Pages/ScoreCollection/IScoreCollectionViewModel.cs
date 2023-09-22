using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;
using BoyumFoosballStats.Models;
using NumSharp.Utilities;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public interface IScoreCollectionViewModel : IViewModelBase
{
    public bool DrawerOpen { get; set; }
    bool ShowInactivePlayers { get; set; }
    bool AutoBalanceMatches { get; set; }
    bool AutoSwapPlayers { get; set; }
    IEnumerable<Player> AvailablePlayers { get; set; }
    HashSet<Player>? SelectedPlayers { get; set; }
    void ToggleDrawer();
    Task LoadPlayers();
}