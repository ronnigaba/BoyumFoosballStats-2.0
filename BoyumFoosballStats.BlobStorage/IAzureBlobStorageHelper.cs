namespace BoyumFoosballStats.BlobStorage;

public interface IAzureBlobStorageHelper
{
    Task<List<T>> UploadListAsync<T>(List<T> entries, string fileName, bool overwrite = false);
    Task<List<T>> GetEntriesAsync<T>(string fileName);
    Task<List<T>> RemoveEntryAsync<T>(T entry, string fileName);
    Task<Stream?> GetFileStreamAsync(string fileName);
    Task UploadFileStreamAsync(string fileName, Stream stream, bool overwrite = false);
}