namespace BoyumFoosballStats;

public class ScoreCollectionViewModel : IScoreCollectionViewModel
{
    public bool DrawerOpen { get; set; } = false;
    public bool ShowInactivePlayers { get; set; }
    public bool AutoBalanceMatches { get; set; }
    public bool AutoSwapPlayers { get; set; }

    public void ToggleDrawer()
    {
        DrawerOpen = !DrawerOpen;
    }
}