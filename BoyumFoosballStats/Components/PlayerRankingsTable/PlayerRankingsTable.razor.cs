using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BoyumFoosballStats.Components.PlayerRankingsTable;

public partial class PlayerRankingsTable
{
    [Parameter] public List<Player> Players { get; set; }

    [Parameter] public EventCallback<Player> SelectedPlayerChanged { get; set; }

    [Parameter] public Player? SelectedPlayer { get; set; }

    private double MaxTrueSkill { get; set; }
    private double MaxGames { get; set; }
    private int SelectedRowNumber { get; set; } = -1;

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
}