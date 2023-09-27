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
        var blobOptions = Options.Create(new BlobStorageOptions()
        {
            BlobUrl = "https://boyumfoosballstorage.blob.core.windows.net/foosballmatches?sp=racwdli&st=2021-11-11T19:06:13Z&se=2099-11-12T03:06:13Z&sv=2020-08-04&sr=c&sig=d%2Fa9iPG41lR54QcBwi1Cy16PVfUac7D2oPTi4ZDQVC0%3D",
            ContainerName = "foosballmatches"
        });
        var outcomeModel = new MatchOutcomeModel(new AzureBlobStorageHelper(blobOptions));
        var result = await outcomeModel.Predict(sampleData);
        Assert.NotNull(result.Score);
        Assert.True(result.Score > 0.0f);
    }
}