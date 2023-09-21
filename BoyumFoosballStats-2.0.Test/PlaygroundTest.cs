using BoyumFoosballStats_2._0.Services;
using Xunit;

namespace BoyumFoosballStats_2._0.Test;

//For random testing / proof of concept logic
public class PlaygroundTest
{
    [Fact]
    public async void Should_PredictFairMatch_When_CalledWithRandomPlayers()
    {
        var matchMakingService = new MatchMakingService();
        var players = await new PlayerCrudCrudService().ReadAllAsync();
        
        var fairMatch = await matchMakingService.FindFairestMatch(players.OrderBy(n => Guid.NewGuid()).Take(5).ToList());

        var blackAttacker = players.Single(x => x.Id == fairMatch.BlackAttackerPlayer.Id).Name;
        var blackDefender = players.Single(x => x.Id == fairMatch.BlackDefenderPlayer.Id).Name;
        var grayAttacker = players.Single(x => x.Id == fairMatch.GrayAttackerPlayer.Id).Name;
        var grayDefender = players.Single(x => x.Id == fairMatch.GrayDefenderPlayer.Id).Name;
    }
}