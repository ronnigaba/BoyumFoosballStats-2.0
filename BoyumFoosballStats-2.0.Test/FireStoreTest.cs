using BoyumFoosballStats_2._0.Shared;
using Firestore.Controllers;
using Xunit;

namespace BoyumFoosballStats_2._0.Test;

public class FireStoreTest
{
    
    [Fact]
    public async void Should_GetAllPlayersFromFirestore_When_Called()
    {
        var firestore = new FirestoreCrudController<Shared.FirestoreModels.Player>(BoyumFoosballStatsConsts.ProjectName, BoyumFoosballStatsConsts.PlayerCollectionName);
        var players = await firestore.ReadAllAsync();

        Assert.NotNull(players);
        Assert.True(players.Count > 0);
    }
    
    [Fact]
    public async void Should_GetAllMatchesFromFirestore_When_Called()
    {
        var firestore = new FirestoreCrudController<Shared.FirestoreModels.Match>(BoyumFoosballStatsConsts.ProjectName, BoyumFoosballStatsConsts.MatchesCollectionName);
        var matches = await firestore.ReadAllAsync();

        Assert.NotNull(matches);
        Assert.True(matches.Count > 0);
    }
}