using BoyumFoosballStats_2._0.Shared;
using Firestore.Services;
using Xunit;

namespace BoyumFoosballStats_2._0.Test.MigrationFromOldApp;

public class PlayerV1MigrationTest
{
    [Fact]
    public async void MigratePlayersToFirestore()
    {
        var playerController =
            new FirestoreCrudService<Shared.FirestoreModels.Player>(BoyumFoosballStatsConsts.ProjectName, BoyumFoosballStatsConsts.PlayerCollectionName);
        var migrations = new List<Shared.FirestoreModels.Player>();

        var existing = await playerController.ReadAllAsync();
        if (existing.Count > 0)
        {
            throw new Exception("Entries already exist, to avoid data loss, please decide what to do with them");
        }

        foreach (Player player in Enum.GetValues(typeof(Player)))
        {
            var migratedPlayer = new Shared.FirestoreModels.Player();
            migratedPlayer.Active = true;
            migratedPlayer.MatchesPlayed = 0;
            migratedPlayer.TrueSkillRating = 0.0f;
            migratedPlayer.Name = Enum.GetName(player);
            migratedPlayer.LegacyPlayerId = (int)player;
            migrations.Add(await playerController.CreateAsync(migratedPlayer));
        }

        Assert.Equal(Enum.GetValues(typeof(Player)).Length, migrations.Count);
    }
}

public enum Player
{
    Ronni = 0,
    Jeppe = 1,
    Dawda = 2,
    Anders = 3,
    Peter = 4,
    Kincső = 5,
    Jacob = 6,
    Henrik = 7,
    Kim = 8,
    Maria = 9,
    Cristian = 10,
    Alex = 11,
    Paul = 12,
    Adam = 13,
    Andrei = 14,
    Mads = 15,
    Rasmus = 16,
    David = 17,
    Khaled = 18,
    Danjal = 19,
    Tomas = 20
}