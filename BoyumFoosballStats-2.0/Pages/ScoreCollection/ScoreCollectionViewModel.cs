using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared.DbModels;
using CosmosDb.Services;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats_2._0.Pages.ScoreCollection;

public class ScoreCollectionViewModel : IScoreCollectionViewModel
{
    private readonly ICosmosDbCrudService<Player> _playerCrudService;

    public ScoreCollectionViewModel(IPlayerCrudService playerCrudService)
    {
        _playerCrudService = playerCrudService;
    }

    public bool DrawerOpen { get; set; }
    public bool ShowInactivePlayers { get; set; }
    public bool AutoBalanceMatches { get; set; }
    public bool AutoSwapPlayers { get; set; }
    public IEnumerable<Player>? AvailablePlayers { get; set; }
    public HashSet<Player>? SelectedPlayers { get; set; }

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
}