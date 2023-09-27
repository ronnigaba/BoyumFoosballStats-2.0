using System.Collections.Generic;
using BoyumFoosballStats.Components.TeamCard.Models;
using BoyumFoosballStats.Shared.DbModels;
using Microsoft.AspNetCore.Components;

#pragma warning disable BL0007

namespace BoyumFoosballStats.Components.TeamCard;

public partial class TeamCard
{
    [Inject] public ITeamCardViewModel ViewModel { get; set; } = null!;

    [Parameter] public IEnumerable<Player>? PlayersList { get; set; }

    [Parameter] public TeamInfo TeamInfo
    {
        get => ViewModel.TeamInfo;
        set => ViewModel.TeamInfo = value;
    }
    
    [Parameter] public EventCallback<TeamInfo> TeamInfoChanged
    {
        get => ViewModel.TeamInfoChanged;
        set => ViewModel.TeamInfoChanged = value;
    }

    [Parameter] public bool IsFlipped
    {
        get => ViewModel.IsFlipped;
        set => ViewModel.IsFlipped = value;
    }

    [Parameter] public TeamCardType Type
    {
        get => ViewModel.Type;
        set => ViewModel.Type = value;
    }
}