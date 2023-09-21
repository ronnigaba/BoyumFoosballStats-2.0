using BoyumFoosballStats_2._0.Services;
using BoyumFoosballStats_Ai;
using Xunit;

namespace BoyumFoosballStats_2._0.Test;

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
        var outcomeModel = new MatchOutcomeModel();
        var result = await outcomeModel.Predict(sampleData);
        Assert.NotNull(result.Score);
        Assert.True(result.Score > 0.0f);
    }
}