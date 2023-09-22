using Newtonsoft.Json;

namespace CosmosDb.Model;

public class CosmosDbBaseModel
{
    [JsonProperty("id")] public string? Id { get; set; } = Guid.NewGuid().ToString();
}