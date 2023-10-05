using System.Collections.Generic;
using System.Linq;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Models;
using Moserware.Skills;
using Player = BoyumFoosballStats.Shared.DbModels.Player;

namespace BoyumFoosballStats.Services.Extensions;

public static class MatchExtensions
{
    public static void UpdateTrueSkill(this Match match)
    {
        if (!match.IsValid())
        {
            return;
        }

        var gameInfo = GameInfo.DefaultGameInfo;
        var blackAttacker = new Moserware.Skills.Player(match.BlackAttackerPlayer?.Id);
        var blackDefender = new Moserware.Skills.Player(match.BlackDefenderPlayer?.Id);
        var greyAttacker = new Moserware.Skills.Player(match.GreyAttackerPlayer?.Id);
        var greyDefender = new Moserware.Skills.Player(match.GreyDefenderPlayer?.Id);

        var blackTeam = new Team()
            .AddPlayer(blackAttacker, match.BlackAttackerPlayer?.TrueSkillRating ?? gameInfo.DefaultRating)
            .AddPlayer(blackDefender, match.BlackDefenderPlayer?.TrueSkillRating ?? gameInfo.DefaultRating);

        var greyTeam = new Team()
            .AddPlayer(greyAttacker, match.GreyAttackerPlayer?.TrueSkillRating ?? gameInfo.DefaultRating)
            .AddPlayer(greyDefender, match.GreyDefenderPlayer?.TrueSkillRating ?? gameInfo.DefaultRating);

        var teams = Teams.Concat(blackTeam, greyTeam);
        var blackRank = match.ScoreBlack > match.ScoreGrey ? 1 : 2;
        var grayRank = match.ScoreGrey > match.ScoreBlack ? 1 : 2;
        var newRatings = TrueSkillCalculator.CalculateNewRatings(gameInfo, teams, blackRank, grayRank);

        match.BlackAttackerPlayer!.TrueSkillRating = new TrueSkillRating(newRatings[blackAttacker]);
        match.BlackDefenderPlayer!.TrueSkillRating = new TrueSkillRating(newRatings[blackDefender]);
        match.GreyAttackerPlayer!.TrueSkillRating = new TrueSkillRating(newRatings[greyAttacker]);
        match.GreyDefenderPlayer!.TrueSkillRating = new TrueSkillRating(newRatings[greyDefender]);
    }

    public static void UpdateMatchesPlayed(this Match match)
    {
        if (!match.IsValid())
        {
            return;
        }

        match.BlackAttackerPlayer!.MatchesPlayed++;
        match.BlackDefenderPlayer!.MatchesPlayed++;
        match.GreyAttackerPlayer!.MatchesPlayed++;
        match.GreyDefenderPlayer!.MatchesPlayed++;
    }

    public static Dictionary<string, PlayerMatchStats> GetPlayerWinRates(this List<Match> matches)
    {
        var winRates = new Dictionary<string, PlayerMatchStats>();

        foreach (var match in matches)
        {
            foreach (var player in match.Players)
            {
                if (player == null)
                {
                    continue;
                }

                var playerName = player.Name;
                winRates.TryAdd(playerName, new PlayerMatchStats());
                winRates[playerName].MatchesPlayed++;
                var playerWon = match.Winners.Contains(player);
                winRates[playerName].MatchesWon = playerWon ? winRates[playerName].MatchesWon + 1 : winRates[playerName].MatchesWon;
                winRates[playerName].MatchesLost = !playerWon ? winRates[playerName].MatchesLost + 1 : winRates[playerName].MatchesLost;
            }
        }

        return winRates;
    }
}

public class PlayerMatchStats
{
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public double WinRate => (double)MatchesWon / MatchesPlayed;
}