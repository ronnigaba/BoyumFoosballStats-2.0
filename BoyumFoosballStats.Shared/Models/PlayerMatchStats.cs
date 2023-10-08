namespace BoyumFoosballStats.Shared.Models;

public class PlayerMatchStats
{
    public int MatchesPlayed { get; set; }
    public int MatchesPlayedAttacker { get; set; }
    public int MatchesPlayedDefender => MatchesPlayed - MatchesPlayedAttacker;
    
    public int MatchesWon { get; set; }
    public int MatchesWonAttacker { get; set; }
    public int MatchesWonDefender => MatchesWon - MatchesWonAttacker;
    
    public int MatchesLost => MatchesPlayed - MatchesWon;
    public int MatchesLostAttacker => MatchesPlayedAttacker - MatchesWonAttacker;
    public int MatchesLostDefender => MatchesPlayedDefender - MatchesWonDefender;
    
    public double WinRate => (double)MatchesWon / MatchesPlayed;
    public double WinRateAttacker => (double)MatchesWonAttacker / MatchesPlayedAttacker;
    public double WinRateDefender => (double)MatchesWonDefender / MatchesPlayedDefender;
}