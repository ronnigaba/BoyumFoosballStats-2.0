using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using BoyumFoosballStats.Enums;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BoyumFoosballStats.Components.PlayerRankingsTable;

public partial class PlayerRankingsTable
{
    [Parameter] public List<Player> Players { get; set; }

    [Parameter] public EventCallback<Player> SelectedPlayerChanged { get; set; }
    [Parameter] public EventCallback<PlayerPosition> SelectedPositionChanged { get; set; }

    [Parameter] public Player? SelectedPlayer { get; set; }

    private double MaxTrueSkill { get; set; }
    private double MaxGames { get; set; }
    private int SelectedRowNumber { get; set; } = -1;
    public PlayerPosition SelectedPosition { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MaxTrueSkill = Players.First().TrueSkillRating!.Mean;
        MaxGames = (double)Players.MaxBy(x => x.MatchesPlayed)!.MatchesPlayed!;
    }

    private async Task HandlePlayerClicked(TableRowClickEventArgs<Player> args)
    {
        SelectedPlayer = args.Item;
        await SelectedPlayerChanged.InvokeAsync(args.Item);
    }


    private int GetRankingNumber(Player player)
    {
        return Players.IndexOf(player) + 1;
    }

    private string SelectedRowClassFunc(Player player, int rowNumber)
    {
        if (SelectedPlayer != null & SelectedPlayer?.Id == player.Id)
        {
            SelectedRowNumber = rowNumber;
            return "row-selected";
        }

        return string.Empty;
    }

    private double GetTrueSkillMean(Player context)
    {
        switch (SelectedPosition)
        {
            case PlayerPosition.Overall:
                return context.TrueSkillRating!.Mean;
            case PlayerPosition.Attacker:
                return context.TrueSkillRatingAttacker!.Mean;
            case PlayerPosition.Defender:
                return context.TrueSkillRatingDefender!.Mean;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private int? GetmatchesPlayed(Player context)
    {
        switch (SelectedPosition)
        {
            case PlayerPosition.Overall:
                return context.MatchesPlayed;
            case PlayerPosition.Attacker:
                return context.MatchesPlayedAttacker;
            case PlayerPosition.Defender:
                return context.MatchesPlayedDefender;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task PlayerPositionChanged(PlayerPosition position)
    {
        SelectedPosition = position;
        
        switch (SelectedPosition)
        {
            case PlayerPosition.Overall:
                MaxTrueSkill = Players.MaxBy(x => x.TrueSkillRating.Mean)!.TrueSkillRating.Mean!;
                MaxGames = (double)Players.MaxBy(x => x.MatchesPlayed)!.MatchesPlayed!;
                break;
            case PlayerPosition.Attacker:
                MaxTrueSkill = Players.MaxBy(x => x.TrueSkillRatingAttacker.Mean)!.TrueSkillRatingAttacker.Mean!;
                MaxGames = (double)Players.MaxBy(x => x.MatchesPlayedAttacker)!.MatchesPlayedAttacker!;
                break;
            case PlayerPosition.Defender:
                MaxTrueSkill = Players.MaxBy(x => x.TrueSkillRatingDefender.Mean)!.TrueSkillRatingDefender.Mean!;
                MaxGames = (double)Players.MaxBy(x => x.MatchesPlayedDefender)!.MatchesPlayedDefender!;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        await SelectedPositionChanged.InvokeAsync(position);
        StateHasChanged();
    }
}