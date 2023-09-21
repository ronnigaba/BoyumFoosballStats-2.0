using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using Firestore.Model;

namespace BoyumFoosballStats_2._0.Shared.FirestoreModels;

[FirestoreData]
public class Match : FirestoreBaseModel
{
    [FirestoreProperty("MatchDate")] public DateTime MatchDate { get; set; }

    [FirestoreProperty("LegacyMatchId")] public string? LegacyMatchId { get; set; }

    [FirestoreProperty("BlackAttackerPlayer")]
    public Player? BlackAttackerPlayer { get; set; }

    [FirestoreProperty("BlackDefenderPlayer")]
    public Player? BlackDefenderPlayer { get; set; }

    [FirestoreProperty("GrayAttackerPlayer")]
    public Player? GrayAttackerPlayer { get; set; }

    [FirestoreProperty("GrayDefenderPlayer")]
    public Player? GrayDefenderPlayer { get; set; }

    [FirestoreProperty("ScoreBlack")]
    [Range(0, 11, ErrorMessage = "Invalid score, valid values are 0-10")]
    public int ScoreBlack { get; set; }

    [FirestoreProperty("ScoreGray")]
    [Range(0, 11, ErrorMessage = "Invalid score, valid values are 0-10")]
    public int ScoreGray { get; set; }
    
    public bool IsValid()
    {
        var players = new List<Player>() { BlackAttackerPlayer, BlackDefenderPlayer, GrayAttackerPlayer, GrayDefenderPlayer };
        if (players.Any(x => x == null) || players.GroupBy(x => x).Any(y => y.Count() > 1))
        {
            return false;
        }

        return true;
    }
}