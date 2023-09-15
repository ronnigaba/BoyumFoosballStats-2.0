using Google.Cloud.Firestore;

namespace BoyumFoosballStats_2._0.Shared.FirestoreModels;

[FirestoreData]
public class Player
{
    [FirestoreDocumentId] public string? Id { get; set; }

    [FirestoreProperty("Name")] public string? Name { get; set; }

    [FirestoreProperty("Active")] public bool Active { get; set; }

    [FirestoreProperty("TrueSkillRating")] public float? TrueSkillRating { get; set; }

    [FirestoreProperty("MatchesPlayed")] public int? MatchesPlayed { get; set; }

    [FirestoreProperty("LegacyPlayerId")] public int? LegacyPlayerId { get; set; }
}