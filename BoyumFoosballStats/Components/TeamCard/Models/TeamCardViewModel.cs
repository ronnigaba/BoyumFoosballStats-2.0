using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Shared.DbModels;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace BoyumFoosballStats.Components.TeamCard.Models;

public class TeamCardViewModel : ITeamCardViewModel
{
    public bool IsFlipped { get; set; }
    public TeamCardType Type { get; set; } = TeamCardType.Light;
    public TeamInfo TeamInfo { get; set; }
    public EventCallback<TeamInfo> TeamInfoChanged { get; set; }

    public string WrapperClasses =>
        new CssBuilder("pa-4 d-flex justify-space-between gap-4")
            .AddClass("flex-column-reverse", when: IsFlipped)
            .AddClass("flex-column", when: !IsFlipped)
            .AddClass("mud--theme-dark", when: Type == TeamCardType.Dark)
            .Build();

    public IEnumerable<Player>? PlayersList { get; set; }

    public string GetDefenderName()
    {
        return $"{TeamInfo.TeamName} Defender";
    }

    public string GetAttackerName()
    {
        return $"{TeamInfo.TeamName} Attacker";
    }

    public async Task IncrementScore()
    {
        TeamInfo.Score += 1;
        await TeamInfoChanged.InvokeAsync(TeamInfo);
    }

    public async Task DecrementScore()
    {
        if (TeamInfo.Score <= 0)
        {
            return;
        }

        TeamInfo.Score -= 1;
        await TeamInfoChanged.InvokeAsync(TeamInfo);
    }


    public async Task HandleDefenderChanged(Player defender)
    {
        TeamInfo.Defender = defender;
        await TeamInfoChanged.InvokeAsync(TeamInfo);
    }

    public async Task HandleAttackerChanged(Player attacker)
    {
        TeamInfo.Attacker = attacker;
        await TeamInfoChanged.InvokeAsync(TeamInfo);
    }
}