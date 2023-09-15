using Firestore;
using Newtonsoft.Json;
using Xunit;

namespace BoyumFoosballStats_2._0.Test.MigrationFromOldApp;

public class MatchesV1MigrationTest
{
    [Fact]
    public async void MigrateMatchesToFirestore()
    {
        var jsonPath = "D:\\Downloads\\Chrome\\matches.json";

        var matches = new List<Match>();

        JsonSerializer serializer = new JsonSerializer();
        using (StreamReader file = File.OpenText(jsonPath))
        {
            matches = (List<Match>)serializer.Deserialize(file, typeof(List<Match>));
        }

        var matchController =
            new FirestoreCrudController<Shared.FirestoreModels.Match>("boyum-foosball-stats", "matches");
        var playerController =
            new FirestoreCrudController<Shared.FirestoreModels.Player>("boyum-foosball-stats", "players");
        var players = await playerController.ReadAllAsync();

        if (matches != null)
        {
            foreach (var oldMatch in matches)
            {
                var migratedMatch = new Shared.FirestoreModels.Match
                {
                    ScoreGray = oldMatch.ScoreGray,
                    ScoreBlack = oldMatch.ScoreBlack,
                    BlackAttackerPlayerId = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Black.Attacker).Id,
                    BlackDefenderPlayerId = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Black.Defender).Id,
                    GrayAttackerPlayerId = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Gray.Attacker).Id,
                    GrayDefenderPlayerId = players.Single(x => x.LegacyPlayerId == (int)oldMatch.Gray.Defender).Id,
                    MatchDate = oldMatch.MatchDate.ToUniversalTime(),
                    LegacyMatchId = oldMatch.Id
                };

                migratedMatch = await matchController.CreateAsync(migratedMatch);
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
    Gray = 1
}