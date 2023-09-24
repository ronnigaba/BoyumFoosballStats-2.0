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