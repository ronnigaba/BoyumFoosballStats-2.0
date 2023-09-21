using BoyumFoosballStats_2._0.Shared.FirestoreModels;
using BoyumFoosballStats_Ai;
using BoyumFoosballStats.Controller;
using MudBlazor.Extensions;

namespace BoyumFoosballStats_2._0.Services;

public class MatchMakingService : IMatchMakingService
{
    public MatchMakingService()
    {
    }

    //ToDo RGA - Return complex object that includes the fairness score - Possibly return all matches in fairness order
    public async Task<Match> FindFairestMatch(List<Player> players)
    {
        var fairestMatch = new Match();
        var bestFairnessFactor = double.MaxValue;
        var predictionModel = new MatchOutcomeModel();

        var combinations = CollectionCombinationHelper.GetAllCombinations(players, 2).ToList();
        foreach (var comb1 in combinations)
        {
            foreach (var comb2 in combinations)
            {
                var match = new Match
                {
                    BlackAttackerPlayer = comb1.First(),
                    BlackDefenderPlayer = comb1.Last(),
                    GrayAttackerPlayer = comb2.First(),
                    GrayDefenderPlayer = comb2.Last()
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
                var result = await predictionModel.Predict(sampleData);
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
}