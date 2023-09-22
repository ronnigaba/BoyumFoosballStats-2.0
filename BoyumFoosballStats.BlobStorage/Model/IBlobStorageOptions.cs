namespace BoyumFoosballStats.BlobStorage.Model;

public interface IBlobStorageOptions
{
    string? BlobUrl { get; set; }
    string? ContainerName { get; set; }
}