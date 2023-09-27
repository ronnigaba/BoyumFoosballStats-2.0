namespace CosmosDb.Services;

public interface ICosmosDbCrudService<T>
{
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateOrUpdateAsync(T item);
    Task CreateOrUpdateAsync(List<T> items);
    Task DeleteAsync(string id);
}