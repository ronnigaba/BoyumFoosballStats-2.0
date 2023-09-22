namespace CosmosDb.Model;

public class CosmosDbSettings : ICosmosDbSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}