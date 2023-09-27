using Newtonsoft.Json;

namespace CosmosDb.Model;

public class CosmosDbBaseModel
{
    [JsonProperty("DsId")] public string? PartitionKey { get; set; }
    [JsonProperty("id")] public string? Id { get; set; } = Guid.NewGuid().ToString();
}