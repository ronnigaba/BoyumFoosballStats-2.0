using BoyumFoosballStats_2._0.Shared.FirestoreModels;
using Firestore;
using Xunit;
using Player = BoyumFoosballStats_2._0.Test.MigrationFromOldApp.Player;

namespace BoyumFoosballStats_2._0.Test;

public class FireStoreTest
{
    [Fact]
    public async void Should_GetAllPlayersFromFirestore_When_Called()
    {
        var firestore = new FirestoreCrudController<Player>("boyum-foosball-stats", "players");
        var players = await firestore.ReadAllAsync();
    }
}