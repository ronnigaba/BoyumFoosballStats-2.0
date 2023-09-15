using BoyumFoosballStats.Models;

namespace BoyumFoosballStats;

public interface IScoreCollectionViewModel : IViewModelBase
{
    public bool DrawerOpen { get; set; }
    void ToggleDrawer();
}