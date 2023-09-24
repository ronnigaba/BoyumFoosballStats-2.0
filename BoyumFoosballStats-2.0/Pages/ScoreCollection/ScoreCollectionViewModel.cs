using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared.DbModels;
using BoyumFoosballStats_2.Components.TeamCard.ViewModel;
using CosmosDb.Services;
using MudBlazor;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public class ScoreCollectionViewModel : IScoreCollectionViewModel
{
    private readonly ICosmosDbCrudService<Player> _playerCrudService;
    private readonly ISnackbar _snackbarService;

    public ScoreCollectionViewModel(IPlayerCrudService playerCrudService, ISnackbar snackbarService)
    {
        _playerCrudService = playerCrudService;
        _snackbarService = snackbarService;
        _snackbarService.Configuration.PositionClass = Defaults.Classes.Position.BottomEnd;
        _snackbarService.Configuration.VisibleStateDuration = 2000;
    }

    public bool DrawerOpen { get; set; }
    public bool ShowInactivePlayers { get; set; }
    public bool AutoBalanceMatches { get; set; }
    public bool AutoSwapPlayers { get; set; }
    public IEnumerable<Player>? AvailablePlayers { get; set; }
    public IEnumerable<Player>? SelectedPlayers { get; set; } = new HashSet<Player>();

    public TeamInfo GreyTeam { get; set; } = new()
    {
        TeamName = "Grey",
        Score = 5
    };

    public TeamInfo BlackTeam { get; set; } = new()
    {
        TeamName = "Black",
        Score = 5
    };

    public void ToggleDrawer()
    {
        DrawerOpen = !DrawerOpen;
    }

    public async Task LoadPlayers()
    {
        var players = await _playerCrudService.GetAllAsync();
        if (!ShowInactivePlayers)
        {
            players = players.Where(x => x.Active);
        }

        AvailablePlayers = players;
    }

    public string? PlayerToString(Player player)
    {
        return player.Name;
    }

    public async Task ShowActiveCheckedChanged(bool arg)
    {
        ShowInactivePlayers = arg;
        await LoadPlayers();
    }

    public Task SaveMatch()
    {
        _snackbarService.Clear();
        _snackbarService.Add("Match saved. GG!",  Severity.Success);
        return Task.CompletedTask;
    }

    public void HandleSelectedPlayersChanged(IEnumerable<Player> selectedPlayers)
    {
        var selectedPlayersList = selectedPlayers.ToList();
        SelectedPlayers = selectedPlayersList;

        if (GreyTeam.Attacker != null && selectedPlayersList.All(p => p.Id != GreyTeam.Attacker.Id))
        {
            GreyTeam = GreyTeam with { Attacker = null };
        }        
        
        if (GreyTeam.Defender != null && selectedPlayersList.All(p => p.Id != GreyTeam.Defender.Id))
        {
            GreyTeam = GreyTeam with { Defender = null };
        }
        
        if (BlackTeam.Attacker != null && selectedPlayersList.All(p => p.Id != BlackTeam.Attacker.Id))
        {
            GreyTeam = BlackTeam with { Attacker = null };
        }        
        
        if (BlackTeam.Defender != null && selectedPlayersList.All(p => p.Id != BlackTeam.Defender.Id))
        {
            BlackTeam = BlackTeam with { Defender = null };
        }
    }
}