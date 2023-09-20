using System.Text;
using Google.Cloud.Storage.V1;

namespace FireStorage.Services;

public class FireStorageService : IFireStorageService
{
    private readonly string _bucketName;

    public FireStorageService(string bucketName)
    {
        _bucketName = bucketName;
    }

    public async Task<Stream> GetFileStream(string fileName)
    {
        var storage = await StorageClient.CreateAsync();
        Stream stream = new MemoryStream();
        await storage.DownloadObjectAsync(_bucketName, fileName, stream);
        return stream;
    }

    public async Task<Object> UploadObjectFromMemory(string objectName, Stream stream)
    {
        var storage = await StorageClient.CreateAsync();
        return await storage.UploadObjectAsync(_bucketName, objectName, "application/octet-stream", stream);
    }

    public async Task DeleteFile(string objectName)
    {
        var storage = await StorageClient.CreateAsync();
        await storage.DeleteObjectAsync(_bucketName, objectName);
    }
}

public interface IFireStorageService
{
    Task<Stream> GetFileStream(string fileName);
    Task<object> UploadObjectFromMemory(string objectName, Stream stream);
    Task DeleteFile(string objectName);
}