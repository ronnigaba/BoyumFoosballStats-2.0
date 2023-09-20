using BoyumFoosballStats_2._0.Shared;
using FireStorage.Services;
using Xunit;

namespace BoyumFoosballStats_2._0.Test;

public class FireStorageServiceTest
{
    [Fact]
    public async void Should_GetAiPredictionModelFromStorage_When_Called()
    {
        var fireStorageService = new FireStorageService(BoyumFoosballStatsConsts.DefaultBucketName);
        var fileStream = await fireStorageService.GetFileStream(BoyumFoosballStatsConsts.AiModelName);
        Assert.NotNull(fileStream);
        Assert.True(fileStream.Length > 0);
    }
    
    [Fact]
    public async void Should_UploadFile_When_Called()
    {
        var fireStorageService = new FireStorageService(BoyumFoosballStatsConsts.DefaultBucketName);
        var fileStream = await fireStorageService.GetFileStream(BoyumFoosballStatsConsts.AiModelName);
        var fileName = "Should_UploadFile_When_Called_File";
        var uploadedObject = await fireStorageService.UploadObjectFromMemory(fileName, fileStream);
        await fireStorageService.DeleteFile(fileName);
        Assert.NotNull(uploadedObject);
    }
}