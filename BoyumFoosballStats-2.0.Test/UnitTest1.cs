using BoyumFoosballStats_2._0.Shared.FirestoreModels;
using Firestore;

namespace BoyumFoosballStats_2._0.Test;

public class UnitTest1
{
    [Fact]
    public async void Test1()
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Users\\rga\\Downloads\\boyum-foosball-stats-firebase-adminsdk-5uxh1-73429a3147.json");
        var firestore = new FirestoreCrudController<Player>("boyum-foosball-stats", "players");
        var players = await firestore.ReadAllAsync();
    }
}