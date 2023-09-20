using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using Firestore.Model;

namespace BoyumFoosballStats_2._0.Shared.FirestoreModels;

[FirestoreData]
public class Match : FirestoreBaseModel
{
    [FirestoreProperty("MatchDate")] public DateTime MatchDate { get; set; }

    [FirestoreProperty("LegacyMatchId")] public string? LegacyMatchId { get; set; }

    [FirestoreProperty("BlackAttackerPlayerId")]
    public string? BlackAttackerPlayerId { get; set; }

    [FirestoreProperty("BlackDefenderPlayerId")]
    public string? BlackDefenderPlayerId { get; set; }

    [FirestoreProperty("GrayAttackerPlayerId")]
    public string? GrayAttackerPlayerId { get; set; }

    [FirestoreProperty("GrayDefenderPlayerId")]
    public string? GrayDefenderPlayerId { get; set; }

    [FirestoreProperty("ScoreBlack")]
    [Range(0, 11, ErrorMessage = "Invalid score, valid values are 0-10")]
    public int ScoreBlack { get; set; }

    [FirestoreProperty("ScoreGray")]
    [Range(0, 11, ErrorMessage = "Invalid score, valid values are 0-10")]
    public int ScoreGray { get; set; }
    
    public bool IsValid()
    {
        var players = new List<string?>() { BlackAttackerPlayerId, BlackDefenderPlayerId, GrayAttackerPlayerId, GrayDefenderPlayerId };
        if (players.Any(x => x == null) || players.GroupBy(x => x).Any(y => y.Count() > 1))
        {
            return false;
        }

        return true;
    }
}