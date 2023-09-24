using BoyumFoosballStats_2._0.Shared.DbModels;

namespace BoyumFoosballStats_2.Components.TeamCard.ViewModel;

public record TeamInfo
{
    public string? TeamName { get; set; }
    public Player? Attacker { get; set; }
    public Player? Defender { get; set; }
    public int Score { get; set; }
}