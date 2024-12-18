using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Models;
using Moserware.Skills;

namespace BoyumFoosballStats.Shared.Extensions;

public static class MatchExtensions
{
    public static IEnumerable<IGrouping<string, Match>> GroupBySeason(this List<Match> matches)
    {
        return matches.GroupBy(x => x.MatchDate.ToString("yyyy/MM"));
    }

    public static void UpdateTrueSkill(this Match match)
    {
        UpdateOverallTrueSkill(match);
        UpdatePositionTrueSkill(match);
        UpdatePositionTrueSkillSeason(match);
        UpdateTrueSkillSeason(match);
    }

    private static void UpdatePositionTrueSkillSeason(this Match match)
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

        var seasonKey = match.MatchDate.GetSeasonKey();
        if (!match.BlackAttackerPlayer!.SeasonalTrueSkill.ContainsKey(seasonKey))
        {
            match.BlackAttackerPlayer!.SeasonalTrueSkill[seasonKey] = new TrueSkillRatings();
        }
        var blackAttackerRating = match.BlackAttackerPlayer?.SeasonalTrueSkill[seasonKey];
        if (!match.BlackDefenderPlayer!.SeasonalTrueSkill.ContainsKey(seasonKey))
        {
            match.BlackDefenderPlayer!.SeasonalTrueSkill[seasonKey] = new TrueSkillRatings();
        }
        var blackDefenderRating = match.BlackAttackerPlayer?.SeasonalTrueSkill[seasonKey];
        var blackTeam = new Team()
            .AddPlayer(blackAttacker, blackAttackerRating?.Attacker ?? gameInfo.DefaultRating)
            .AddPlayer(blackDefender, blackDefenderRating?.Defender ?? gameInfo.DefaultRating);

        if (!match.GreyAttackerPlayer!.SeasonalTrueSkill.ContainsKey(seasonKey))
        {
            match.GreyAttackerPlayer!.SeasonalTrueSkill[seasonKey] = new TrueSkillRatings();
        }
        var greyAttackerRating = match.BlackAttackerPlayer?.SeasonalTrueSkill[seasonKey];
        if (!match.GreyDefenderPlayer!.SeasonalTrueSkill.ContainsKey(seasonKey))
        {
            match.GreyDefenderPlayer!.SeasonalTrueSkill[seasonKey] = new TrueSkillRatings();
        }
        var greyDefenderRating = match.BlackAttackerPlayer?.SeasonalTrueSkill[seasonKey];
        var greyTeam = new Team()
            .AddPlayer(greyAttacker, greyAttackerRating?.Attacker ?? gameInfo.DefaultRating)
            .AddPlayer(greyDefender, greyDefenderRating?.Defender ?? gameInfo.DefaultRating);

        var teams = Teams.Concat(blackTeam, greyTeam);
        var blackRank = match.ScoreBlack > match.ScoreGrey ? 1 : 2;
        var grayRank = match.ScoreGrey > match.ScoreBlack ? 1 : 2;
        var newRatings = TrueSkillCalculator.CalculateNewRatings(gameInfo, teams, blackRank, grayRank);

        match.BlackAttackerPlayer!.SeasonalTrueSkill[seasonKey]!.Attacker = new TrueSkillRating(newRatings[blackAttacker]);
        match.BlackDefenderPlayer!.SeasonalTrueSkill[seasonKey]!.Defender = new TrueSkillRating(newRatings[blackDefender]);
        match.GreyAttackerPlayer!.SeasonalTrueSkill[seasonKey]!.Attacker = new TrueSkillRating(newRatings[greyAttacker]);
        match.GreyDefenderPlayer!.SeasonalTrueSkill[seasonKey]!.Defender = new TrueSkillRating(newRatings[greyDefender]);
    }
    private static void UpdateTrueSkillSeason(this Match match)
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

        var seasonKey = match.MatchDate.GetSeasonKey();
        if (!match.BlackAttackerPlayer!.SeasonalTrueSkill.ContainsKey(seasonKey))
        {
            match.BlackAttackerPlayer!.SeasonalTrueSkill[seasonKey] = new TrueSkillRatings();
        }
        var blackAttackerRating = match.BlackAttackerPlayer?.SeasonalTrueSkill[seasonKey];
        if (!match.BlackDefenderPlayer!.SeasonalTrueSkill.ContainsKey(seasonKey))
        {
            match.BlackDefenderPlayer!.SeasonalTrueSkill[seasonKey] = new TrueSkillRatings();
        }
        var blackDefenderRating = match.BlackAttackerPlayer?.SeasonalTrueSkill[seasonKey];
        var blackTeam = new Team()
            .AddPlayer(blackAttacker, blackAttackerRating?.Overall ?? gameInfo.DefaultRating)
            .AddPlayer(blackDefender, blackDefenderRating?.Overall ?? gameInfo.DefaultRating);

        if (!match.GreyAttackerPlayer!.SeasonalTrueSkill.ContainsKey(seasonKey))
        {
            match.GreyAttackerPlayer!.SeasonalTrueSkill[seasonKey] = new TrueSkillRatings();
        }
        var greyAttackerRating = match.BlackAttackerPlayer?.SeasonalTrueSkill[seasonKey];
        if (!match.GreyDefenderPlayer!.SeasonalTrueSkill.ContainsKey(seasonKey))
        {
            match.GreyDefenderPlayer!.SeasonalTrueSkill[seasonKey] = new TrueSkillRatings();
        }
        var greyDefenderRating = match.BlackAttackerPlayer?.SeasonalTrueSkill[seasonKey];
        var greyTeam = new Team()
            .AddPlayer(greyAttacker, greyAttackerRating?.Overall ?? gameInfo.DefaultRating)
            .AddPlayer(greyDefender, greyDefenderRating?.Overall ?? gameInfo.DefaultRating);

        var teams = Teams.Concat(blackTeam, greyTeam);
        var blackRank = match.ScoreBlack > match.ScoreGrey ? 1 : 2;
        var grayRank = match.ScoreGrey > match.ScoreBlack ? 1 : 2;
        var newRatings = TrueSkillCalculator.CalculateNewRatings(gameInfo, teams, blackRank, grayRank);

        match.BlackAttackerPlayer!.SeasonalTrueSkill[seasonKey]!.Overall = new TrueSkillRating(newRatings[blackAttacker]);
        match.BlackDefenderPlayer!.SeasonalTrueSkill[seasonKey]!.Overall = new TrueSkillRating(newRatings[blackDefender]);
        match.GreyAttackerPlayer!.SeasonalTrueSkill[seasonKey]!.Overall = new TrueSkillRating(newRatings[greyAttacker]);
        match.GreyDefenderPlayer!.SeasonalTrueSkill[seasonKey]!.Overall = new TrueSkillRating(newRatings[greyDefender]);
    }

    private static void UpdatePositionTrueSkill(this Match match)
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
            .AddPlayer(blackAttacker, match.BlackAttackerPlayer?.TrueSkillRatingAttacker ?? gameInfo.DefaultRating)
            .AddPlayer(blackDefender, match.BlackDefenderPlayer?.TrueSkillRatingDefender ?? gameInfo.DefaultRating);

        var greyTeam = new Team()
            .AddPlayer(greyAttacker, match.GreyAttackerPlayer?.TrueSkillRatingAttacker ?? gameInfo.DefaultRating)
            .AddPlayer(greyDefender, match.GreyDefenderPlayer?.TrueSkillRatingDefender ?? gameInfo.DefaultRating);

        var teams = Teams.Concat(blackTeam, greyTeam);
        var blackRank = match.ScoreBlack > match.ScoreGrey ? 1 : 2;
        var grayRank = match.ScoreGrey > match.ScoreBlack ? 1 : 2;
        var newRatings = TrueSkillCalculator.CalculateNewRatings(gameInfo, teams, blackRank, grayRank);

        match.BlackAttackerPlayer!.TrueSkillRatingAttacker = new TrueSkillRating(newRatings[blackAttacker]);
        match.BlackDefenderPlayer!.TrueSkillRatingDefender = new TrueSkillRating(newRatings[blackDefender]);
        match.GreyAttackerPlayer!.TrueSkillRatingAttacker = new TrueSkillRating(newRatings[greyAttacker]);
        match.GreyDefenderPlayer!.TrueSkillRatingDefender = new TrueSkillRating(newRatings[greyDefender]);
    }

    private static void UpdateOverallTrueSkill(this Match match)
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
        match.BlackAttackerPlayer!.MatchesPlayedAttacker++;
        match.BlackDefenderPlayer!.MatchesPlayed++;
        match.BlackDefenderPlayer!.MatchesPlayedDefender++;
        match.GreyAttackerPlayer!.MatchesPlayed++;
        match.GreyAttackerPlayer!.MatchesPlayedAttacker++;
        match.GreyDefenderPlayer!.MatchesPlayed++;
        match.GreyDefenderPlayer!.MatchesPlayedDefender++;
    }
}