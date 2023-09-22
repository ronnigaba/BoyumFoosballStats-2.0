using BoyumFoosballStats_2._0.Services;
using BoyumFoosballStats_2._0.Shared;
using CosmosDb.Model;
using Microsoft.Extensions.Options;
using Xunit;

namespace BoyumFoosballStats_2._0.Test.MigrationFromOldApp;

public class PlayerV1MigrationTest
{
    [Fact]
    public async void MigratePlayersToFirestore()
    {
        var cosmos = new CosmosDbSettings()
        {
            ConnectionString =
                "AccountEndpoint=https://boyum-foosball-stats.documents.azure.com:443/;AccountKey=bzFDKIaIPRtg20nSvj60LtLujmZLRLInZwQzZk3jMaB2H1qiljTeT38y3JTkZtHx4vBCInSp5unrACDbDgrE1w==",
            DatabaseName = "BoyumFoosballStats"
        };
        var options = Options.Create(cosmos);
        var playerController =
            new PlayerCrudService(options);
        var migrations = new List<Shared.DbModels.Player>();

        var existing = await playerController.GetAllAsync();
        if (existing.Any())
        {
            throw new Exception("Entries already exist, to avoid data loss, please decide what to do with them");
        }

        foreach (Player player in Enum.GetValues(typeof(Player)))
        {
            var migratedPlayer = new Shared.DbModels.Player
            {
                Active = true,
                MatchesPlayed = 0,
                TrueSkillRating = 0.0f,
                Name = Enum.GetName(player),
                LegacyPlayerId = (int)player
            };
            migrations.Add(await playerController.CreateOrUpdateAsync(migratedPlayer));
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