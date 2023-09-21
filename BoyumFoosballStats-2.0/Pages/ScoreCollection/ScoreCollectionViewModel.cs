using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared.FirestoreModels;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public class ScoreCollectionViewModel : IScoreCollectionViewModel
{
    private readonly IPlayerCrudService _playerService;

    public ScoreCollectionViewModel(IPlayerCrudService playerService)
    {
        _playerService = playerService;
    }

    public bool DrawerOpen { get; set; } = false;
    public bool ShowInactivePlayers { get; set; }
    public bool AutoBalanceMatches { get; set; }
    public bool AutoSwapPlayers { get; set; }
    public IEnumerable<Player> AvailablePlayers { get; set; } = new List<Player>();
    public IEnumerable<Player>? SelectedPlayers { get; set; }

    public async void ToggleDrawer()
    {
        DrawerOpen = !DrawerOpen;
        AvailablePlayers = await _playerService.ReadAllAsync();
    }
}