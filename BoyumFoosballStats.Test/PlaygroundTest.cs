using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BoyumFoosballStats.Ai;
using BoyumFoosballStats.BlobStorage;
using BoyumFoosballStats.BlobStorage.Model;
using BoyumFoosballStats.Services;
using BoyumFoosballStats.Shared.DbModels;
using CosmosDb.Model;
using Microsoft.Extensions.Options;
using Xunit;

namespace BoyumFoosballStats.Test;

//For random testing / proof of concept logic
public class PlaygroundTest
{
    [Fact]
    public async void Should_PredictFairMatch_When_CalledWithRandomPlayers()
    {
        // var matchMakingService = new MatchMakingService();
        // var players = await new PlayerCrudService().GetAllAsync();
        //
        // var fairMatch = await matchMakingService.FindFairestMatch(players.OrderBy(n => Guid.NewGuid()).Take(5).ToList());
        //
        // var blackAttacker = players.Single(x => x.Id == fairMatch.BlackAttackerPlayer.Id).Name;
        // var blackDefender = players.Single(x => x.Id == fairMatch.BlackDefenderPlayer.Id).Name;
        // var grayAttacker = players.Single(x => x.Id == fairMatch.GrayAttackerPlayer.Id).Name;
        // var grayDefender = players.Single(x => x.Id == fairMatch.GrayDefenderPlayer.Id).Name;
    }

    [Fact]
    public async void Test()
    {
        var tokenCredential = new DefaultAzureCredential();
        var secretClient = new SecretClient(new Uri("https://boyumfoosballstats.vault.azure.net/"), tokenCredential);
        var blobSettings = new BlobStorageOptions
        {
            BlobUrl = secretClient.GetSecret("BlobStorageConnectionString").Value.Value,
            ContainerName = "BoyumFoosballStats"
        };
        var cosmosSettings = new CosmosDbSettings
        {
            ConnectionString = secretClient.GetSecret("CosmosDbConnectionString").Value.Value,
            DatabaseName = "BoyumFoosballStats"
        };
        var cosmosOptions = Options.Create(cosmosSettings);
        var blobOptions = Options.Create(blobSettings);
        var blobHelper = new AzureBlobStorageHelper(blobOptions);

        var playerCrudService = new PlayerCrudService(cosmosOptions);
        var players = await playerCrudService.GetAllAsync();
        
        var matchCrudService = new MatchCrudService(cosmosOptions);
        var matches = await matchCrudService.GetAllAsync();
        var matchesList = matches.TakeLast(10).ToList();
        var lastMatch = matchesList.Last();
        
        var matchMakingService = new MatchMakingService(blobHelper);
        var playerInMatch = players.Where(x => !lastMatch.Players.Contains(x)).TakeLast(2).ToList();
        playerInMatch.AddRange(lastMatch.Players);
        var swappedMatch = await matchMakingService.AutoSwapPlayers(matchesList, playerInMatch);

    }
}