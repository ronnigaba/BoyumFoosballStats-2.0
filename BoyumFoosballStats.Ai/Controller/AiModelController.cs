using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace BoyumFoosballStats.Ai.Controller;

public class AiModelController<TInputModel, TOutputModel> where TInputModel : class where TOutputModel : class, new()
{
    readonly MLContext _mlContext = new();

    public ExperimentResult<RegressionMetrics> Train(IEnumerable<TInputModel> sourceData, string labelColumn = "Label", uint secondsToTrain = 120, Stream? saveStream = null)
    {
        var context = new MLContext();

        var data = _mlContext.Data.LoadFromEnumerable(sourceData);

        var settings = new RegressionExperimentSettings
        {
            MaxExperimentTimeInSeconds = secondsToTrain,
            OptimizingMetric = RegressionMetric.MeanSquaredError
        };

        var progress = new Progress<RunDetail<RegressionMetrics>>(p =>
        {
            if (p.ValidationMetrics != null)
            {
                //ToDo RGA - turn this into a callback function
                Console.WriteLine($"Current Result - {p.TrainerName}, {p.ValidationMetrics.RSquared}, {p.ValidationMetrics.MeanAbsoluteError}");
            }
        });

        var experiment = context.Auto().CreateRegressionExperiment(settings);
        var result = experiment.Execute(data, labelColumn, progressHandler: progress);
        if (saveStream != null)
        {
            _mlContext.Model.Save(result.BestRun.Model, data.Schema, saveStream);
        }

        return result;
    }

    public TOutputModel Predict(Stream modelStream, TInputModel data)
    {
        var model = _mlContext.Model.Load(modelStream, out var outputSchema);
        return Predict(model, data);
    }

    public TOutputModel Predict(ITransformer model, TInputModel data)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<TInputModel, TOutputModel>(model);
        return predictionEngine.Predict(data);
    }
}