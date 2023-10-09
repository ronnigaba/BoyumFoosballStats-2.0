using System.Collections.Generic;
using System.Threading.Tasks;
using BoyumFoosballStats.Shared.DbModels;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.Components.TeamCard.Models;

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
    IEnumerable<Player>? PlayersList { get; set; }
}