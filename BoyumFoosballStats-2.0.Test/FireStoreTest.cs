using Firestore.Controllers;
using Xunit;

namespace BoyumFoosballStats_2._0.Test;

public class FireStoreTest
{
    [Fact]
    public async void Should_GetAllPlayersFromFirestore_When_Called()
    {
        var firestore = new FirestoreCrudController<Shared.FirestoreModels.Player>("boyum-foosball-stats", "players");
        var players = await firestore.ReadAllAsync();
    }
}