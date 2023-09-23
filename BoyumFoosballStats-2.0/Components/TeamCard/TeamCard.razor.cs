using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;
using BoyumFoosballStats_2.Components.TeamCard.ViewModel;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

#pragma warning disable BL0007

namespace BoyumFoosballStats_2.Components.TeamCard;

public partial class TeamCard
{
    [Inject] public ITeamCardViewModel ViewModel { get; set; } = null!;

    [Parameter]
    public string? TeamName
    {
        get => ViewModel.TeamName;
        set => ViewModel.TeamName = value;
    }

    [Parameter] public IEnumerable<Player>? PlayersList { get; set; }

    [Parameter]
    public int Score
    {
        get => ViewModel.Score;
        set => ViewModel.Score = value;
    }

    [Parameter]
    public EventCallback<int> ScoreChanged
    {
        get => ViewModel.ScoreChanged;
        set => ViewModel.ScoreChanged = value;
    }

    [Parameter] public Player? Attacker { get; set; }
    [Parameter] public Player? Defender { get; set; }
    [Parameter] public EventCallback<Player> DefenderChanged { get; set; }
    [Parameter] public EventCallback<Player> AttackerChanged { get; set; }
    [Parameter] public bool IsFlipped { get; set; }

    [Parameter] public TeamCardType Type { get; set; }

    private async Task HandleDefenderChanged(Player defender)
    {
        Defender = defender;
        await DefenderChanged.InvokeAsync(defender);
    }

    private async Task HandleAttackerChanged(Player attacker)
    {
        Attacker = attacker;
        await AttackerChanged.InvokeAsync(attacker);
    }

    // protected override void OnAfterRender(bool firstRender)
    // {
    //     base.OnAfterRender(firstRender);
    //
    //     if (Attacker != null && PlayersList!.All(x => x.Id != Attacker.Id))
    //     {
    //         Attacker = null;
    //     }
    //     
    //     if (Defender != null && PlayersList!.All(x => x.Id != Defender.Id))
    //     {
    //         Defender = null;
    //     }
    // }

    public string WrapperClasses =>
        new CssBuilder("pa-4 d-flex justify-space-between gap-4")
            .AddClass("flex-column-reverse", when: IsFlipped)
            .AddClass("flex-column", when: !IsFlipped)
            .AddClass("mud--theme-dark", when: Type == TeamCardType.Dark)
            .Build();
}