using Google.Cloud.Firestore;

namespace Firestore.Controllers;

public interface IFirestoreCrudController<T>
{
    Task DeleteAsync(string documentId);
    Task<T> UpdateAsync(string documentId, T updatedDocument);
    Task<List<T>> ReadAllAsync(Filter? filter = null, string? orderByField = null);
    Task<T?> ReadAsync(string documentId);
    Task<T> CreateAsync(T document);
}