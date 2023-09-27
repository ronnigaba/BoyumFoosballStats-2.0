using CosmosDb.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace CosmosDb.Services;

public class CosmosDbCrudService<T> : ICosmosDbCrudService<T> where T : CosmosDbBaseModel
{
    private readonly string _partitionKey;
    private readonly Container _container;

    public CosmosDbCrudService(IOptions<CosmosDbSettings> dbSettings, string containerName, string partitionKey)
    {
        _partitionKey = partitionKey;
        var client = new CosmosClient(dbSettings.Value.ConnectionString);
        var database = client.GetDatabase(dbSettings.Value.DatabaseName);
        _container = database.GetContainer(containerName);
    }

    public async Task<T> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(id, new PartitionKey(_partitionKey));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default(T);
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<T>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<T> CreateOrUpdateAsync(T item)
    {
        return await _container.UpsertItemAsync(item);
    }

    public async Task CreateOrUpdateAsync(List<T> items)
    {
        if (items == null || items.Count == 0)
        {
            return;
        }

        var partitionKey = new PartitionKey(_partitionKey);
        var batch = _container.CreateTransactionalBatch(partitionKey);

        foreach (var item in items)
        {
            batch.UpsertItem(item);
        }

        await batch.ExecuteAsync();
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<T>(id, new PartitionKey(_partitionKey));
    }
}