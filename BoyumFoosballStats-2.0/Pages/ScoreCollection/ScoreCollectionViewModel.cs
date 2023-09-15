namespace BoyumFoosballStats;

public class ScoreCollectionViewModel : IScoreCollectionViewModel
{
    public bool DrawerOpen { get; set; } = false;

    public void ToggleDrawer()
    {
        DrawerOpen = !DrawerOpen;
    }
}