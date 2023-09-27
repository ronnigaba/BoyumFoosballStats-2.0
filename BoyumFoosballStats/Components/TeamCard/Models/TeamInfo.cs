using BoyumFoosballStats.Shared.DbModels;

namespace BoyumFoosballStats.Components.TeamCard.Models;

public record TeamInfo
{
    public string? TeamName { get; set; }
    public Player? Attacker { get; set; }
    public Player? Defender { get; set; }
    public int Score { get; set; }
}