using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats_2.Components.TeamCard.ViewModel;

public interface ITeamCardViewModel
{
    string? TeamName { get; set; }
    int Score { get; set; }
    Task DecrementScore();
    Task IncrementScore();
    public EventCallback<int> ScoreChanged { get; set; }
    public bool IsFlipped { get; set; }
    TeamCardType Type { get; set; }
    string GetDefenderName();
    string GetAttackerName();
}