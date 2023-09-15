using Google.Cloud.Firestore;

namespace BoyumFoosballStats_2._0.Shared.FirestoreModels;

[FirestoreData]
public class Player
{
    [FirestoreProperty] public string? Name { get; set; }
    [FirestoreProperty] public bool Active { get; set; }
    [FirestoreProperty] public float? EloRank { get; set; }
    [FirestoreProperty] public int? MatchesPlayed { get; set; }
}