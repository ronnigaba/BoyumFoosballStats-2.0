using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared.DbModels;
using CosmosDb.Services;
using Microsoft.AspNetCore.Components;
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
    public int GreyScore { get; set; } = 5;
    public int BlackScore { get; set; } = 5;
    public Player? SelectedBlackAttacker { get; set; }
    public Player? SelectedBlackDefender { get; set; }
    public Player? SelectedGreyAttacker { get; set; }
    public Player? SelectedGreyDefender { get; set; }

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
}