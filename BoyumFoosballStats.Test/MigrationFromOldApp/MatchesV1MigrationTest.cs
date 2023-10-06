using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BoyumFoosballStats.BlobStorage;
using BoyumFoosballStats.BlobStorage.Model;
using BoyumFoosballStats.Services;
using BoyumFoosballStats.Shared.Extensions;
using CosmosDb.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace BoyumFoosballStats.Test.MigrationFromOldApp;

public class MatchesV1MigrationTest
{
    [Fact]
    public async void MigrateMatchesToFirestore()
    {
        var tokenCredential = new DefaultAzureCredential();
        var secretClient = new SecretClient(new Uri("https://boyumfoosballstats.vault.azure.net/"), tokenCredential);
        var cosmosSettings = new CosmosDbSettings
        {
            ConnectionString = secretClient.GetSecret("CosmosDbConnectionString").Value.Value,
            DatabaseName = "BoyumFoosballStats"
        };
        var cosmosOptions = Options.Create(cosmosSettings);
       var blobSettings = new BlobStorageOptions
        {
            BlobUrl = secretClient.GetSecret("BlobStorageConnectionString").Value.Value,
            ContainerName = "BoyumFoosballStats"
        };
        var blobOptions = Options.Create(blobSettings);
        var blobHelper = new AzureBlobStorageHelper(blobOptions);
        var matches = await blobHelper.GetEntriesAsync<Match>("matches.json");
        var matchController = new MatchCrudService(cosmosOptions);
        var playerController = new PlayerCrudService(cosmosOptions);
        var players = await playerController.GetAllAsync();

        if (matches != null)
        {
            foreach (var oldMatch in matches)
            {
                var blackAttackerPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Black.Attacker!);
                var blackDefenderPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Black.Defender!);
                var greyAttackerPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Gray.Attacker!);
                var greyDefenderPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Gray.Defender!);
                var migratedMatch = new Shared.DbModels.Match
                {
                    ScoreGrey = oldMatch.ScoreGray,
                    ScoreBlack = oldMatch.ScoreBlack,
                    BlackAttackerPlayer = blackAttackerPlayer,
                    BlackDefenderPlayer = blackDefenderPlayer,
                    GreyAttackerPlayer = greyAttackerPlayer,
                    GreyDefenderPlayer = greyDefenderPlayer,
                    MatchDate = oldMatch.MatchDate.ToUniversalTime(),
                    LegacyMatchId = oldMatch.Id
                };
                migratedMatch.UpdateMatchesPlayed();
                migratedMatch.UpdateTrueSkill();
                await matchController.CreateOrUpdateAsync(migratedMatch);
            }
        }

        foreach (var player in players)
        {
            await playerController.CreateOrUpdateAsync(player);
        }

        var test = matches;
    }
}

public class Match
{
    public Match()
    {
        Black = new Team(TableSide.Black);
        Gray = new Team(TableSide.Grey);
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }

    public Team Black { get; set; }
    public Team Gray { get; set; }

    public int ScoreBlack { get; set; }

    public int ScoreGray { get; set; }

    public DateTime MatchDate { get; set; }
}

public class Team : TeamBase
{
    public Team(TableSide side)
    {
        Side = side;
    }

    public TableSide Side { get; set; }
}

public class TeamBase
{
    public Player? Attacker { get; set; }

    public Player? Defender { get; set; }

    [JsonIgnore] public List<Player?> Players => new List<Player?>() { Attacker, Defender };

    [JsonIgnore] public string TeamIdentifier => $"{Attacker} {Defender}";
    [JsonIgnore] public string TeamIdentifierSwapped => $"{Defender} {Attacker}";
}

public enum TableSide
{
    Black = 0,
    Grey = 1
}