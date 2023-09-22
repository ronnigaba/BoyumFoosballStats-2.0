using System.Text.Json;
using Azure.Storage.Blobs;
using BoyumFoosballStats.BlobStorage.Model;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats.BlobStorage
{
    public class AzureBlobStorageHelper : IAzureBlobStorageHelper
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string? _containerName;

        public AzureBlobStorageHelper(IOptions<BlobStorageOptions> options)
        {
            _blobServiceClient = new BlobServiceClient(new Uri(options.Value.BlobUrl));
            _containerName = options.Value.ContainerName;
        }

        public async Task<List<T>> UploadListAsync<T>(List<T> entries, string fileName, bool overwrite = false)
        {
            var entriesToUpload = new List<T>();
            BlobClient blobClient = GetBlobClient(fileName);
            if (!overwrite)
            {
                entriesToUpload.AddRange(await GetEntriesAsync<T>(fileName));
            }

            entriesToUpload.AddRange(entries);
            var localFilePath = $"./{fileName}";
            var json = JsonSerializer.Serialize(entriesToUpload);
            await File.WriteAllTextAsync(localFilePath, json);

            await blobClient.UploadAsync(localFilePath, true);
            return entriesToUpload;
        }

        public async Task<List<T>> GetEntriesAsync<T>(string fileName)
        {
            BlobClient blobClient = GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var blobResult = await blobClient.DownloadContentAsync();
                var entries = JsonSerializer.Deserialize<List<T>>(blobResult.Value.Content);
                if (entries != null && entries.Any())
                {
                    return entries;
                }
            }

            return new List<T>();
        }

        public async Task<List<T>> RemoveEntryAsync<T>(T entry, string fileName)
        {
            var entries = await GetEntriesAsync<T>(fileName);
            entries.Remove(entry);
            await UploadListAsync(entries, fileName, true);
            return entries;
        }

        public async Task<Stream?> GetFileStreamAsync(string fileName)
        {
            BlobClient blobClient = GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var blobResult = await blobClient.DownloadContentAsync();
                return blobResult.Value.Content.ToStream();
            }

            return null;
        }

        public async Task UploadFileStreamAsync(string fileName, Stream stream, bool overwrite = false)
        {
            BlobClient blobClient = GetBlobClient(fileName);

            if (!overwrite)
            {
                BlobClient backupBlobClient = GetBlobClient($"{fileName}_old");
                var oldFile = await GetFileStreamAsync(fileName);
                await backupBlobClient.UploadAsync(oldFile);
            }

            await stream.FlushAsync();
            stream.Position = 0;
            await blobClient.UploadAsync(stream, true);
        }

        private BlobClient GetBlobClient(string fileName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            return containerClient.GetBlobClient(fileName);
        }
    }
}

public interface IAzureBlobStorageHelper
{
    Task<List<T>> UploadListAsync<T>(List<T> entries, string fileName, bool overwrite = false);
    Task<List<T>> GetEntriesAsync<T>(string fileName);
    Task<List<T>> RemoveEntryAsync<T>(T entry, string fileName);
    Task<Stream?> GetFileStreamAsync(string fileName);
    Task UploadFileStreamAsync(string fileName, Stream stream, bool overwrite = false);
}