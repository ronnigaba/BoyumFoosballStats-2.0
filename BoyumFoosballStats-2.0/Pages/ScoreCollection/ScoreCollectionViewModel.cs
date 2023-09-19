using BoyumFoosballStats_2._0.Services.Interface;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public class ScoreCollectionViewModel : IScoreCollectionViewModel
{
    public bool DrawerOpen { get; set; } = false;
    public bool ShowInactivePlayers { get; set; }
    public bool AutoBalanceMatches { get; set; }
    public bool AutoSwapPlayers { get; set; }
    public IEnumerable<string> AvailablePlayers { get; set; } = new List<string>() { "Ronni", "Jeppe", "Alex", "Peter", "Ronni", "Jeppe", "Alex", "Peter", "Ronni", "Jeppe", "Alex", "Peter"};
    public IEnumerable<string>? SelectedPlayers { get; set; }

    public void ToggleDrawer()
    {
        DrawerOpen = !DrawerOpen;
    }
}