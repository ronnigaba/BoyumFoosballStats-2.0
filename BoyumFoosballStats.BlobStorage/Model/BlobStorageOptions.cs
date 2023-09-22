namespace BoyumFoosballStats.BlobStorage.Model;

public class BlobStorageOptions : IBlobStorageOptions
{
    public string? BlobUrl { get; set; }
    public string? ContainerName { get; set; }
}