using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace BoyumFoosballStats_2.Components.TeamCard.ViewModel;

public class TeamCardViewModel : ITeamCardViewModel
{
    public string? TeamName { get; set; }
    public int Score { get; set; } = 5;
    public EventCallback<int> ScoreChanged { get; set; }
    public bool IsFlipped { get; set; }
    public TeamCardType Type { get; set; } = TeamCardType.Light;

    public string GetDefenderName()
    {
        return $"{TeamName} Defender";
    }

    public string GetAttackerName()
    {
        return $"{TeamName} Attacker";
    }

    public async Task IncrementScore()
    {
        Score += 1;
        await ScoreChanged.InvokeAsync(Score);
    }

    public async Task DecrementScore()
    {
        if (Score <= 0)
        {
            return;
        }

        Score -= 1;
        await ScoreChanged.InvokeAsync(Score);
    }
}