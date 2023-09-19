using Google.Cloud.Firestore;

namespace Firestore.Model;

public class FirestoreBaseModel
{
    [FirestoreDocumentId] public string? Id { get; set; }
}