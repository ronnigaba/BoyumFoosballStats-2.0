using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats_2._0.Shared.DbModels;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats_2.Components.TeamCard.ViewModel;

public interface ITeamCardViewModel
{
    Task DecrementScore();
    Task IncrementScore();
    public bool IsFlipped { get; set; }
    TeamCardType Type { get; set; }
    TeamInfo TeamInfo { get; set; }
    EventCallback<TeamInfo> TeamInfoChanged { get; set; }
    string GetDefenderName();
    string GetAttackerName();
    Task HandleDefenderChanged(Player defender);
    Task HandleAttackerChanged(Player attacker);
    string WrapperClasses { get; }
}