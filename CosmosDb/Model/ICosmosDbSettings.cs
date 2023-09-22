namespace CosmosDb.Model;

public interface ICosmosDbSettings
{
    string? ConnectionString { get; set; }
    string? DatabaseName { get; set; }
}