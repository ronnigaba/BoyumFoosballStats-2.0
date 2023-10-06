﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Components.TeamCard.Models;
using BoyumFoosballStats.Enums;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Enums;
using BoyumFoosballStats.Shared.Extensions;
using CosmosDb.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;

namespace BoyumFoosballStats.Pages.ScoreCollection.Models;

public class ScoreCollectionViewModel : IScoreCollectionViewModel
{
    private readonly ICosmosDbCrudService<Player> _playerCrudService;
    private readonly ISnackbar _snackbarService;
    private readonly IMatchMakingService _matchMakingService;
    private readonly IMatchCrudService _matchCrudService;
    private readonly ISessionCrudService _sessionCrudService;
    private readonly ProtectedLocalStorage _protectedLocalStorage;

    public ScoreCollectionViewModel(IPlayerCrudService playerCrudService, ISnackbar snackbarService,
        IMatchMakingService matchMakingService, IMatchCrudService matchCrudService,
        ISessionCrudService sessionCrudService, ProtectedLocalStorage protectedLocalStorage)
    {
        _playerCrudService = playerCrudService;
        _matchCrudService = matchCrudService;
        _sessionCrudService = sessionCrudService;
        _protectedLocalStorage = protectedLocalStorage;
        _snackbarService = snackbarService;
        _matchMakingService = matchMakingService;
        _matchCrudService = matchCrudService;
        _snackbarService.Configuration.PositionClass = Defaults.Classes.Position.BottomEnd;
        _snackbarService.Configuration.VisibleStateDuration = 2000;
    }

    private Session ActiveSession { get; set; } = new();
    public IEnumerable<Player>? AvailablePlayers { get; set; }
    public IEnumerable<Player> SelectedPlayers { get; set; } = new List<Player>();
    public bool IsSessionActive { get; set; }

    public TeamInfo GreyTeam { get; set; } = new()
    {
        TeamName = BoyumFoosballStatsConsts.GreyTeamName,
        Score = 5
    };

    public TeamInfo BlackTeam { get; set; } = new()
    {
        TeamName = BoyumFoosballStatsConsts.BlackTeamName,
        Score = 5
    };


    public async Task LoadPlayers()
    {
        var players = await _playerCrudService.GetAllAsync();
        AvailablePlayers = players;
    }

    public async Task SaveMatch()
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
            return;
        }

        match.UpdateMatchesPlayed();
        match.UpdateTrueSkill();
        await _matchCrudService.CreateOrUpdateAsync(match);
        _snackbarService.Add("Match saved. GG!", Severity.Success);
        await _playerCrudService.CreateOrUpdateAsync(new List<Player>
        {
            match.BlackDefenderPlayer!, match.BlackAttackerPlayer!, match.GreyAttackerPlayer!, match.GreyDefenderPlayer!
        });
        GreyTeam.Score = 5;
        BlackTeam.Score = 5;
        await SaveSessionIfActive(match);
    }

    public async Task HandleSelectedPlayersChanged(IEnumerable<Player> selectedPlayers)
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

        await SaveSessionIfActive();
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

    public async Task TeamInfoChanged(TeamInfo teamInfo)
    {
        if (teamInfo.TeamName == BoyumFoosballStatsConsts.GreyTeamName)
        {
            GreyTeam = teamInfo;
        }
        else
        {
            BlackTeam = teamInfo;
        }

        await SaveSessionIfActive();
    }

    public async Task LoadSession()
    {
        var sessionRetrieval =
            await _protectedLocalStorage.GetAsync<string>(BoyumFoosballStatsConsts.SessionIdLocalStorageKey);
        if (sessionRetrieval is { Success: true, Value: not null })
        {
            var sessionById = await _sessionCrudService.GetByIdAsync(sessionRetrieval.Value);
            if (sessionById != null)
            {
                IsSessionActive = true;
                ActiveSession = sessionById;
                GreyTeam.Attacker = AvailablePlayers?.SingleOrDefault(x=>x.Id == ActiveSession.GreyAttackerId);
                GreyTeam.Defender = AvailablePlayers?.SingleOrDefault(x=>x.Id ==ActiveSession.GreyDefenderId);
                BlackTeam.Attacker = AvailablePlayers?.SingleOrDefault(x=>x.Id ==ActiveSession.BlackAttackerId);
                BlackTeam.Defender = AvailablePlayers?.SingleOrDefault(x=>x.Id ==ActiveSession.BlackDefenderId);
                SelectedPlayers = AvailablePlayers?.Where(x=> ActiveSession.SelectedPlayers.Contains(x.Id)).ToList()!;
            }
        }
    }

    public async Task ActivateSessionChanged(bool arg)
    {
        IsSessionActive = arg;
        if (!IsSessionActive && ActiveSession.Id != null)
        {
            ActiveSession.State = SessionState.Closed;
            ActiveSession.EndDate = DateTime.Now;
            await _sessionCrudService.CreateOrUpdateAsync(ActiveSession);
            await _protectedLocalStorage.DeleteAsync(BoyumFoosballStatsConsts.SessionIdLocalStorageKey);
            _snackbarService.Add("Session closed!", Severity.Info);
        }
        else
        {
            _snackbarService.Add("Session started!", Severity.Info);
        }
    }

    private async Task SaveSessionIfActive(Match? match = null)
    {
        if (!IsSessionActive)
        {
            return;
        }

        ActiveSession.GreyDefenderId = GreyTeam.Defender?.Id;
        ActiveSession.GreyAttackerId = GreyTeam.Attacker?.Id;
        ActiveSession.BlackDefenderId = BlackTeam.Defender?.Id;
        ActiveSession.BlackAttackerId = BlackTeam.Attacker?.Id;
        ActiveSession.SelectedPlayers = SelectedPlayers.Select(x=>x.Id).ToList();
        if (match != null)
        {
            ActiveSession.Matches.Add(match);
        }

        var session = await _sessionCrudService.CreateOrUpdateAsync(ActiveSession);
        if (session.Id != null)
        {
            await _protectedLocalStorage.SetAsync(BoyumFoosballStatsConsts.SessionIdLocalStorageKey, session.Id);
        }
    }
}