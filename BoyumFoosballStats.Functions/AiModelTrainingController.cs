using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BoyumFoosballStats.Ai;
using BoyumFoosballStats.Ai.Controller;
using BoyumFoosballStats.BlobStorage;
using BoyumFoosballStats.BlobStorage.Model;
using BoyumFoosballStats.Cosmos.Services;
using CosmosDb.Model;
using Microsoft.Extensions.Options;

namespace BoyumFoosballStats.Functions;

public class AiModelTrainingController
{
    public async Task TrainAiModel(uint seconds)
    {
        var tokenCredential = new DefaultAzureCredential();
        var secretClient = new SecretClient(new Uri("https://boyumfoosballstats.vault.azure.net/"), tokenCredential);
        var cosmosSettings = new CosmosDbSettings
        {
            ConnectionString = secretClient.GetSecret("CosmosDbConnectionString").Value.Value,
            DatabaseName = "BoyumFoosballStats"
        };
        var cosmosOptions = Options.Create(cosmosSettings);
        var matchController = new MatchCrudService(cosmosOptions);
        var matches = await matchController.GetAllAsync();
        var predictionMatches = matches.Select(x => new MatchOutcomeModel.ModelInput
        {
            GrayDefender = x.GreyDefenderPlayer.Id,
            GrayAttacker = x.GreyAttackerPlayer.Id,
            BlackDefender = x.BlackDefenderPlayer.Id,
            BlackAttacker = x.BlackAttackerPlayer.Id,
            Winner = x.WinningSide
        });
        var aiController = new AiModelController<MatchOutcomeModel.ModelInput, MatchOutcomeModel.ModelOutput>();
        var blobSettings = new BlobStorageOptions
        {
            BlobUrl = secretClient.GetSecret(BlobStorageConstants.BlobStorageConnectionStringSecretKey).Value.Value,
            ContainerName = BlobStorageConstants.ContainerName
        };
        var blobOptions = Options.Create(blobSettings);
        var blobHelper = new AzureBlobStorageHelper(blobOptions);
        await using (Stream stream = new MemoryStream())
        {
            aiController.Train(predictionMatches, nameof(MatchOutcomeModel.ModelInput.Winner), seconds, stream);
            var oldModel = await blobHelper.GetFileStreamAsync(BlobStorageConstants.MatchOutcomeModel);
            if (oldModel != null)
            {
                await blobHelper.UploadFileStreamAsync(BlobStorageConstants.MatchOutcomeModelBackup, oldModel, true);
            }
            await blobHelper.UploadFileStreamAsync(BlobStorageConstants.MatchOutcomeModel, stream, true);
        }
    }
}