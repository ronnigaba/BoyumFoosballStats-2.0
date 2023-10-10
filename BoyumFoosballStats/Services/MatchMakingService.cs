using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Ai;
using BoyumFoosballStats.BlobStorage;
using BoyumFoosballStats.Enums;
using BoyumFoosballStats.Helpers;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using BoyumFoosballStats.Shared.Extensions;
using Moserware.Skills;
using MudBlazor.Extensions;
using Player = BoyumFoosballStats.Shared.DbModels.Player;

namespace BoyumFoosballStats.Services;

public class MatchMakingService : IMatchMakingService
{
    private readonly IAzureBlobStorageHelper _blobStorageHelper;

    public MatchMakingService(IAzureBlobStorageHelper blobStorageHelper)
    {
        _blobStorageHelper = blobStorageHelper;
    }

    public async Task<Match> AutoSwapPlayers(List<Match> matches, List<Player> players,
        MatchMakingMethod? matchMakingMethod = null)
    {
        if (matches.Count == 0)
        {
            //If no matches are supplied - find the fairest match
            return await FindFairestMatch(players, matchMakingMethod ?? MatchMakingMethod.Ai);
        }

        var lastMatch = matches.Last();
        var newMatch = new Match
        {
            BlackAttackerPlayer = lastMatch.BlackAttackerPlayer,
            BlackDefenderPlayer = lastMatch.BlackDefenderPlayer,
            GreyAttackerPlayer = lastMatch.GreyAttackerPlayer,
            GreyDefenderPlayer = lastMatch.GreyDefenderPlayer
        };
        var playersInLastMatch = players.Where(x => lastMatch.Players.Any(y => y.Id == x.Id)).ToList();
        var playersNotInLastMatch = players.Except(playersInLastMatch).ToList();
        if (playersNotInLastMatch.Count >= 4)
        {
            //If all players need to swap - find the fairest match
            return await FindFairestMatch(playersNotInLastMatch, matchMakingMethod ?? MatchMakingMethod.Ai);
        }

        var matchesPlayedByPlayer = playersInLastMatch
            .ToDictionary(
                player => player,
                player => matches
                    .OrderByDescending(match => match.MatchDate)
                    .TakeWhile(match => match.Players.Any(x => x.Id == player.Id))
                    .Count()
            )
            .Where(kv => kv.Value > 0)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
        
        if (matchMakingMethod != null)
        {
            //If a matchmaking method was explicitly specified - find the fairest match
            var playersToKeep = matchesPlayedByPlayer.OrderBy(x => x.Value).Take(4 - playersNotInLastMatch.Count);
            var playersForNewMatch = playersToKeep.Select(x => x.Key).ToList();
            playersForNewMatch.AddRange(playersNotInLastMatch);
            return await FindFairestMatch(playersForNewMatch, matchMakingMethod.Value);
        }

        //If none of the other cases are true - swap each player individually
        var playersToSwap = matchesPlayedByPlayer.OrderByDescending(x => x.Value)
            .ThenBy(x => lastMatch.Winners.Contains(x.Key)).Take(playersNotInLastMatch.Count);
        var swapIndex = 0;
        foreach (var playerToSwap in playersToSwap)
        {
            var swapId = playerToSwap.Key.Id;
            newMatch.SwapPlayer(swapId, playersNotInLastMatch[swapIndex]);
            swapIndex++;
        }

        return newMatch;
    }

    public Task<Match> FindFairestMatch(List<Player> players, MatchMakingMethod method)
    {
        return method switch
        {
            MatchMakingMethod.Ai => FindFairestMatchAi(players),
            MatchMakingMethod.TrueSkill => FindFairestMatchTrueSkill(players),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                "MatchMakingMethod is not currently supported")
        };
    }

    //ToDo RGA - Return complex object that includes the fairness score - Possibly return all matches in fairness order
    private async Task<Match> FindFairestMatchAi(IEnumerable<Player> players)
    {
        var fairestMatch = new Match();
        var bestFairnessFactor = double.MaxValue;
        var outcomeModel = new MatchOutcomeModel(_blobStorageHelper);

        var combinations = CollectionCombinationHelper.GetAllCombinations(players, 2).ToList();
        foreach (var comb1 in combinations)
        {
            foreach (var comb2 in combinations)
            {
                var match = new Match
                {
                    BlackAttackerPlayer = comb1.First(),
                    BlackDefenderPlayer = comb1.Last(),
                    GreyAttackerPlayer = comb2.First(),
                    GreyDefenderPlayer = comb2.Last()
                };
                if (!match.IsValid())
                {
                    continue;
                }

                //ToDo RGA - Move to method that can calculate based on Match instead
                var sampleData = new MatchOutcomeModel.ModelInput
                {
                    GrayDefender = comb1.First().LegacyPlayerId.As<int>(),
                    GrayAttacker = comb1.Last().LegacyPlayerId.As<int>(),
                    BlackDefender = comb2.First().LegacyPlayerId.As<int>(),
                    BlackAttacker = comb2.Last().LegacyPlayerId.As<int>(),
                };
                var result = await outcomeModel.Predict(sampleData);
                double resultDifference = Math.Abs(result.Score * 100 - 50f);
                if (resultDifference < bestFairnessFactor)
                {
                    bestFairnessFactor = resultDifference;
                    fairestMatch = match;
                }
            }
        }

        return fairestMatch;
    }

    private async Task<Match> FindFairestMatchTrueSkill(IEnumerable<Player> players)
    {
        var fairestMatch = new Match();
        double bestFairnessFactor = 0;

        var combinations = CollectionCombinationHelper.GetAllCombinations(players, 2).ToList();
        foreach (var comb1 in combinations)
        {
            foreach (var comb2 in combinations)
            {
                var match = new Match
                {
                    BlackAttackerPlayer = comb1.First(),
                    BlackDefenderPlayer = comb1.Last(),
                    GreyAttackerPlayer = comb2.First(),
                    GreyDefenderPlayer = comb2.Last()
                };
                if (!match.IsValid())
                {
                    continue;
                }

                var blackAttacker = new Moserware.Skills.Player(match.BlackAttackerPlayer?.Id);
                var blackDefender = new Moserware.Skills.Player(match.BlackDefenderPlayer?.Id);
                var greyAttacker = new Moserware.Skills.Player(match.GreyAttackerPlayer?.Id);
                var greyDefender = new Moserware.Skills.Player(match.GreyDefenderPlayer?.Id);

                var blackTeam = new Team()
                    .AddPlayer(blackAttacker, match.BlackAttackerPlayer?.TrueSkillRating)
                    .AddPlayer(blackDefender, match.BlackDefenderPlayer?.TrueSkillRating);

                var greyTeam = new Team()
                    .AddPlayer(greyAttacker, match.GreyAttackerPlayer?.TrueSkillRating)
                    .AddPlayer(greyDefender, match.GreyDefenderPlayer?.TrueSkillRating);

                var teams = Teams.Concat(blackTeam, greyTeam);

                var fairness = TrueSkillCalculator.CalculateMatchQuality(GameInfo.DefaultGameInfo, teams);
                if (fairness > bestFairnessFactor)
                {
                    bestFairnessFactor = fairness;
                    fairestMatch = match;
                }
            }
        }


        return fairestMatch;
    }
}