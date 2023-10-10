using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BoyumFoosballStats.Ai;
using BoyumFoosballStats.BlobStorage;
using BoyumFoosballStats.BlobStorage.Model;
using Microsoft.Extensions.Options;
using Xunit;

namespace BoyumFoosballStats.Test;

public class AiModelControllerTest
{
    [Fact]
    public async void Should_PredictMatch_When_called()
    {
        var sampleData = new MatchOutcomeModel.ModelInput
        {
            GrayDefender = 1,
            GrayAttacker = 2,
            BlackDefender = 3,
            BlackAttacker = 4,
        };
        var tokenCredential = new DefaultAzureCredential();
        var secretClient = new SecretClient(new Uri("https://boyumfoosballstats.vault.azure.net/"), tokenCredential);
        var blobSettings = new BlobStorageOptions
        {
            BlobUrl = secretClient.GetSecret("BlobStorageConnectionString").Value.Value,
            ContainerName = "BoyumFoosballStats"
        };
        var blobOptions = Options.Create(blobSettings);
        var blobHelper = new AzureBlobStorageHelper(blobOptions);
        var outcomeModel = new MatchOutcomeModel(blobHelper);
        var result = await outcomeModel.Predict(sampleData);
        Assert.NotNull(result.Score);
        Assert.True(result.Score > 0.0f);
    }
}