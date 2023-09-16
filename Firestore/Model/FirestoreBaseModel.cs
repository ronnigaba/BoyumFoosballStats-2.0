using Google.Cloud.Firestore;

namespace BoyumFoosballStats_2._0.Shared.FirestoreModels;

public class FirestoreBaseModel
{
    [FirestoreDocumentId] public string? Id { get; set; }
}