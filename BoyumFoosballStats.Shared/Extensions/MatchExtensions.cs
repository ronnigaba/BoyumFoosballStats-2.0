using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Models;
using Moserware.Skills;
using Player = BoyumFoosballStats.Shared.DbModels.Player;

namespace BoyumFoosballStats.Shared.Extensions;

public static class MatchExtensions
{
    public static IEnumerable<IGrouping<string, Match>> GroupBySeason(this List<Match> matches)
    {
        return matches.GroupBy(x => x.MatchDate.ToString("yyyy/MM"));
    }

    public static void SwapPlayer(this Match match, string oldPlayerId, Player newPlayer)
    {
        if (match.BlackAttackerPlayer?.Id == oldPlayerId)
        {
            match.BlackAttackerPlayer = newPlayer;
        }

        if (match.BlackDefenderPlayer?.Id == oldPlayerId)
        {
            match.BlackDefenderPlayer = newPlayer;
        }

        if (match.GreyAttackerPlayer?.Id == oldPlayerId)
        {
            match.GreyAttackerPlayer = newPlayer;
        }

        if (match.GreyDefenderPlayer?.Id == oldPlayerId)
        {
            match.GreyDefenderPlayer = newPlayer;
        }
    }
    
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

    public static void IncrementMatchesPlayed(this Match match)
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
    
    public static bool WasPlayerInMatch(this Match match, string playerId)
    {
        return match.BlackAttackerPlayer?.Id == playerId ||
               match.BlackDefenderPlayer?.Id == playerId ||
               match.GreyAttackerPlayer?.Id == playerId ||
               match.GreyDefenderPlayer?.Id == playerId;
    }
}