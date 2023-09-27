﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Components.TeamCard.Models;
using BoyumFoosballStats.Services.Extensions;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Services;
using MudBlazor;

namespace BoyumFoosballStats.Pages.ScoreCollection;

public class ScoreCollectionViewModel : IScoreCollectionViewModel
{
    private readonly ICosmosDbCrudService<Player> _playerCrudService;
    private readonly ISnackbar _snackbarService;
    private readonly IMatchMakingService _matchMakingService;
    private readonly IMatchCrudService _matchCrudService;

    public ScoreCollectionViewModel(IPlayerCrudService playerCrudService, ISnackbar snackbarService,
        IMatchMakingService matchMakingService, IMatchCrudService matchCrudService)
    {
        _playerCrudService = playerCrudService;
        _matchCrudService = matchCrudService;
        _snackbarService = snackbarService;
        _matchMakingService = matchMakingService;
        _matchCrudService = matchCrudService;
        _snackbarService.Configuration.PositionClass = Defaults.Classes.Position.BottomEnd;
        _snackbarService.Configuration.VisibleStateDuration = 2000;
    }

    public IEnumerable<Player>? AvailablePlayers { get; set; }
    public IEnumerable<Player>? SelectedPlayers { get; set; } = new List<Player>();

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

    public async Task LoadPlayers()
    {
        var players = await _playerCrudService.GetAllAsync();
        AvailablePlayers = players;
    }

    public string? PlayerToString(Player player)
    {
        return player.Name;
    }

    public Task SaveMatch()
    {
        _snackbarService.Clear();
        var match = new Match
        {
            BlackAttackerPlayer = BlackTeam.Attacker,
            BlackDefenderPlayer = BlackTeam.Defender,
            ScoreBlack = BlackTeam.Score,
            GreyAttackerPlayer = GreyTeam.Attacker,
            GreyDefenderPlayer = GreyTeam.Defender,
            ScoreGrey = GreyTeam.Score,
            MatchDate = DateTime.Now,
        };

        if (!match.IsValid())
        {
            return Task.CompletedTask;
        }

        match.UpdateMatchesPlayed();
        match.UpdateTrueSkill();
        _matchCrudService.CreateOrUpdateAsync(match);
        _snackbarService.Add("Match saved. GG!", Severity.Success);
        _playerCrudService.CreateOrUpdateAsync(new List<Player>
        {
            match.BlackDefenderPlayer!, match.BlackAttackerPlayer!, match.GreyAttackerPlayer!, match.GreyDefenderPlayer!
        });
        GreyTeam.Score = 5;
        BlackTeam.Score = 5;
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

    public async Task BalanceMatch()
    {
        var players = new List<Player>
            { BlackTeam.Defender!, BlackTeam.Attacker!, GreyTeam.Attacker!, GreyTeam.Defender! };
        if (players.Any())
        {
            var fairMatch = await _matchMakingService.FindFairestMatch(players, MatchMakingMethod.Ai);
            GreyTeam = GreyTeam with
            {
                Attacker = fairMatch.GreyAttackerPlayer, Defender = fairMatch.GreyDefenderPlayer
            };
            BlackTeam = BlackTeam with
            {
                Attacker = fairMatch.BlackAttackerPlayer, Defender = fairMatch.BlackDefenderPlayer
            };
        }
    }
}