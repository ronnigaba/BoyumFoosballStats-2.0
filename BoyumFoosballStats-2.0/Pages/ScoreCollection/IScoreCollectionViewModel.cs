using BoyumFoosballStats.Models;

namespace BoyumFoosballStats;

public interface IScoreCollectionViewModel : IViewModelBase
{
    public bool DrawerOpen { get; set; }
    bool ShowInactivePlayers { get; set; }
    bool AutoBalanceMatches { get; set; }
    bool AutoSwapPlayers { get; set; }
    void ToggleDrawer();
}