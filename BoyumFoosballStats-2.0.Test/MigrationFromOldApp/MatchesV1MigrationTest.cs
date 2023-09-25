using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BoyumFoosballStats_2._0.Services;
using BoyumFoosballStats_2._0.Services.Extensions;
using BoyumFoosballStats_2._0.Shared;
using CosmosDb.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace BoyumFoosballStats_2._0.Test.MigrationFromOldApp;

public class MatchesV1MigrationTest
{
    [Fact]
    public async void MigrateMatchesToFirestore()
    {
        var jsonPath = "D:\\Downloads\\Chrome\\matches.json";
        var tokenCredential = new DefaultAzureCredential();
        var secretClient = new SecretClient(new Uri("https://boyumfoosballstats.vault.azure.net/"), tokenCredential);
        var cosmos = new CosmosDbSettings
        {
            ConnectionString = secretClient.GetSecret("CosmosDbConnectionString").Value.Value,
            DatabaseName = "BoyumFoosballStats"
        };
        var options = Options.Create(cosmos);
        var matches = new List<Match>();

        JsonSerializer serializer = new JsonSerializer();
        using (StreamReader file = File.OpenText(jsonPath))
        {
            matches = (List<Match>)serializer.Deserialize(file, typeof(List<Match>));
        }

        var matchController = new MatchCrudService(options);
        var playerController = new PlayerCrudService(options);
        var players = await playerController.GetAllAsync();

        if (matches != null)
        {
            foreach (var oldMatch in matches)
            {
                var migratedMatch = new Shared.DbModels.Match
                {
                    ScoreGrey = oldMatch.ScoreGrey,
                    ScoreBlack = oldMatch.ScoreBlack,
                    BlackAttackerPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Black.Attacker),
                    BlackDefenderPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Black.Defender),
                    GreyAttackerPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Gray.Attacker),
                    GreyDefenderPlayer = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Gray.Defender),
                    MatchDate = oldMatch.MatchDate.ToUniversalTime(),
                    LegacyMatchId = oldMatch.Id
                };
                migratedMatch.UpdateMatchesPlayed();
                migratedMatch.UpdateTrueSkill();
                migratedMatch = await matchController.CreateOrUpdateAsync(migratedMatch);
            }
        }


        var test = matches;
    }
}

public class Match
{
    public Match()
    {
        Black = new Team(TableSide.Black);
        Gray = new Team(TableSide.Gray);
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }

    public Team Black { get; set; }
    public Team Gray { get; set; }

    public int ScoreBlack { get; set; }

    public int ScoreGrey { get; set; }

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
    Gray = 1
}