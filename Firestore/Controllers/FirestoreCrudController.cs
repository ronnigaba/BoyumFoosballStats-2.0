using Firestore.Model;
using Google.Cloud.Firestore;

namespace Firestore.Controllers;

public class FirestoreCrudController<T> : IFirestoreCrudController<T> where T : FirestoreBaseModel
{
    private FirestoreDb? _db;
    private readonly string _projectId;
    private readonly string _collectionName;

    public FirestoreCrudController(string projectId, string collectionName)
    {
        _projectId = projectId;
        _collectionName = collectionName;
    }

    public async Task<T> CreateAsync(T document)
    {
        await InitializeIfNeeded();
        var docRef = _db.Collection(_collectionName).Document();
        await docRef.SetAsync(document);
        return document;
    }

    public async Task<T?> ReadAsync(string documentId)
    {
        await InitializeIfNeeded();
        var docRef = _db.Collection(_collectionName).Document(documentId);
        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<T>();
        }

        return default;
    }

    public async Task<List<T>> ReadAllAsync(Filter? filter = null, string? orderByField = null)
    {
        await InitializeIfNeeded();
        Query query = _db.Collection(_collectionName);

        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        if (orderByField != null)
        {
            query = query.OrderBy(orderByField);
        }

        var querySnapshot = await query.GetSnapshotAsync();
        return querySnapshot.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
    }

    public async Task<T> UpdateAsync(string documentId, T updatedDocument)
    {
        await InitializeIfNeeded();
        var docRef = _db.Collection(_collectionName).Document(documentId);
        await docRef.SetAsync(updatedDocument, SetOptions.MergeAll);
        return updatedDocument;
    }

    public async Task DeleteAsync(string documentId)
    {
        await InitializeIfNeeded();
        var docRef = _db.Collection(_collectionName).Document(documentId);
        await docRef.DeleteAsync();
    }

    private async Task InitializeIfNeeded()
    {
        if (_db == null)
        {
#if DEBUG
            //ToDo RGA - No magic strings!
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                @"C:\Users\Ronni\RiderProjects\boyum-foosball-stats-firebase-adminsdk-5uxh1-73429a3147.json");
#endif
            _db = await FirestoreDb.CreateAsync(_projectId);
        }
    }
}