using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoyumFoosballStats.Ai;
using BoyumFoosballStats.BlobStorage;
using BoyumFoosballStats.Helpers;
using BoyumFoosballStats.Services.Interface;
using BoyumFoosballStats.Shared.DbModels;
using MudBlazor.Extensions;

namespace BoyumFoosballStats.Services;

public class MatchMakingService : IMatchMakingService
{
    private readonly IAzureBlobStorageHelper _blobStorageHelper;

    public MatchMakingService(IAzureBlobStorageHelper blobStorageHelper)
    {
        _blobStorageHelper = blobStorageHelper;
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
    public async Task<Match> FindFairestMatch(IEnumerable<Player> players)
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
                    GrayDefender = comb1.First().LegacyPlayerId.As<float>(),
                    GrayAttacker = comb1.Last().LegacyPlayerId.As<float>(),
                    BlackDefender = comb2.First().LegacyPlayerId.As<float>(),
                    BlackAttacker = comb2.Last().LegacyPlayerId.As<float>(),
                };
                var result = await outcomeModel.Predict(sampleData);
                double resultDifference = Math.Abs(result.Score - 50f);
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