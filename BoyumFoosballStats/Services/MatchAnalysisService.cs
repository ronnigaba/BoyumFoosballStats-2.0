using System.Collections.Generic;
using System.Linq;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Models;

namespace BoyumFoosballStats.Services;

public class MatchAnalysisService : IMatchAnalysisService
{
    private Dictionary<string, PlayerMatchStats>? PlayerMatchStatsCache { get; set; }
    private Dictionary<string, double>? TableSideWinRateCache { get; set; }

    public void BustCache()
    {
        PlayerMatchStatsCache = null;
        TableSideWinRateCache = null;
    }

    public double GetTrueSkillDeltaFromPreviousMatch(Player player, string? matchId, List<Match> matches)
    {
        var trueSkillChange = 0d;
        if (matches.Any())
        {
            var matchIndex = matches.FindIndex(x => x.Id == matchId);
            for (int i = matchIndex + 1; i <= matches.Count; i++)
            {
                var match = matches[i];
                if (!match.Players.Any(x => x.Id == player.Id))
                {
                    continue;
                }

                var prevMatchPlayer = match.Players.Single(x => x.Id == player.Id);

                trueSkillChange = player.TrueSkillRating!.Mean - prevMatchPlayer!.TrueSkillRating!.Mean;
                break;
            }
        }

        return trueSkillChange;
    }

    public Dictionary<string, double> GetTableSideWinRates(List<Match> matches)
    {
        if (TableSideWinRateCache != null)
        {
            return TableSideWinRateCache;
        }

        var winRates = new Dictionary<string, double>();

        var blackWins = 0;
        var greyWins = 0;
        foreach (var match in matches)
        {
            blackWins = match.ScoreBlack > match.ScoreGrey ? blackWins + 1 : blackWins;
            greyWins = match.ScoreGrey > match.ScoreBlack ? greyWins + 1 : greyWins;
        }

        winRates.TryAdd("Black", (double)blackWins / matches.Count);
        winRates.TryAdd("Grey", (double)greyWins / matches.Count);
        return winRates;
    }

    public Dictionary<string, PlayerMatchStats> GetPlayerMatchStats(List<Match> matches)
    {
        if (PlayerMatchStatsCache != null)
        {
            return PlayerMatchStatsCache;
        }

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

                //MatchesPlayed
                winRates[playerName].MatchesPlayed++;
                var playerWon = match.Winners.Contains(player);

                //MatchesWon
                winRates[playerName].MatchesWon =
                    playerWon ? winRates[playerName].MatchesWon + 1 : winRates[playerName].MatchesWon;

                //MatchesPlayedAttacker
                var playerWasAttacker = match.Attackers.Contains(player);
                winRates[playerName].MatchesPlayedAttacker = playerWasAttacker
                    ? winRates[playerName].MatchesPlayedAttacker + 1
                    : winRates[playerName].MatchesPlayedAttacker;

                //MatchesWonAttacker
                winRates[playerName].MatchesWonAttacker = playerWon && playerWasAttacker
                    ? winRates[playerName].MatchesWonAttacker + 1
                    : winRates[playerName].MatchesWonAttacker;
            }
        }

        return winRates;
    }
}