using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats_2._0.Shared.DbModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BoyumFoosballStats_2.Pages.MatchHistory;

public partial class MatchHistory
{
    public List<Match> Matches { get; set; }

    [Inject] public IMatchCrudService MatchCrudService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Matches = (await MatchCrudService.GetAllAsync()).Reverse().ToList();
    }

    public Color GetGreyScoreColor(Match match)
    {
        return match.ScoreGrey > match.ScoreBlack ? Color.Success : Color.Error;
    }    
    public Color GetBlackScoreColor(Match match)
    {
        return match.ScoreBlack > match.ScoreGrey ? Color.Success : Color.Error;
    }
}