using BoyumFoosballStats_2._0.Shared.DbModels;
using BoyumFoosballStats_2._0.Shared.Models;
using Moserware.Skills;

namespace BoyumFoosballStats_2._0.Services.Extensions;

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

        match.BlackAttackerPlayer!.TrueSkillRating = newRatings[blackAttacker] as TrueSkillRating;
        match.BlackDefenderPlayer!.TrueSkillRating = newRatings[blackDefender] as TrueSkillRating;
        match.GreyAttackerPlayer!.TrueSkillRating = newRatings[greyAttacker] as TrueSkillRating;
        match.GreyDefenderPlayer!.TrueSkillRating = newRatings[greyDefender] as TrueSkillRating;
    }

    public static void UpdateMatchesPlayed(this Match match)
    {
        if (!match.IsValid())
        {
            return;
        }

        match.BlackAttackerPlayer.MatchesPlayed++;
        match.BlackDefenderPlayer.MatchesPlayed++;
        match.GreyAttackerPlayer.MatchesPlayed++;
        match.GreyDefenderPlayer.MatchesPlayed++;
    }
}