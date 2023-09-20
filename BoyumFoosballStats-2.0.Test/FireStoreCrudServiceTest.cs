using BoyumFoosballStats_2._0.Shared;
using Firestore.Services;
using Xunit;

namespace BoyumFoosballStats_2._0.Test;

public class FireStoreCrudServiceTest
{
    
    [Fact]
    public async void Should_GetPlayersFromFirestore_When_Called()
    {
        var firestore = new FirestoreCrudService<Shared.FirestoreModels.Player>(BoyumFoosballStatsConsts.ProjectName, BoyumFoosballStatsConsts.PlayerCollectionName);
        var players = await firestore.ReadAllAsync();

        Assert.NotNull(players);
        Assert.True(players.Count > 0);
    }
    
    [Fact]
    public async void Should_GetMatchesFromFirestore_When_Called()
    {
        var firestore = new FirestoreCrudService<Shared.FirestoreModels.Match>(BoyumFoosballStatsConsts.ProjectName, BoyumFoosballStatsConsts.MatchesCollectionName);
        var matches = await firestore.ReadAllAsync();

        Assert.NotNull(matches);
        Assert.True(matches.Count > 0);
    }

}