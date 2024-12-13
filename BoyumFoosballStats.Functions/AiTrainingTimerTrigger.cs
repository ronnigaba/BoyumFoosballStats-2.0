using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BoyumFoosballStats.Functions;

public static class AiTrainingTimerTrigger
{
    [FunctionName("AiTrainingTimerTrigger")]
    public static async Task RunAsync([TimerTrigger("0 0 12 * * SAT")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        var aiModelTrainingController = new AiModelTrainingController();
        await aiModelTrainingController.TrainAiModel(250);
    }
}