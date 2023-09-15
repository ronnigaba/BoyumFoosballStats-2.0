﻿using Google.Cloud.Firestore;

namespace Firestore;

public class FirestoreCrudController<T>
{
    private readonly FirestoreDb _db;
    private readonly string _collectionName;

    public FirestoreCrudController(string projectId, string collectionName)
    {
        _db = FirestoreDb.Create(projectId);
        _collectionName = collectionName;
    }

    public async Task<T> CreateAsync(T document, string documentId)
    {
        var docRef = _db.Collection(_collectionName).Document(documentId);
        await docRef.SetAsync(document);
        return document;
    }

    public async Task<T?> ReadAsync(string documentId)
    {
        var docRef = _db.Collection(_collectionName).Document(documentId);
        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<T>();
        }
        else
        {
            return default(T);
        }
    }

    public async Task<List<T>> ReadAllAsync()
    {
        var query = _db.Collection(_collectionName);
        var querySnapshot = await query.GetSnapshotAsync();
        return querySnapshot.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
    }

    public async Task<T> UpdateAsync(string documentId, T updatedDocument)
    {
        var docRef = _db.Collection(_collectionName).Document(documentId);
        await docRef.SetAsync(updatedDocument, SetOptions.MergeAll);
        return updatedDocument;
    }

    public async Task DeleteAsync(string documentId)
    {
        var docRef = _db.Collection(_collectionName).Document(documentId);
        await docRef.DeleteAsync();
    }
}