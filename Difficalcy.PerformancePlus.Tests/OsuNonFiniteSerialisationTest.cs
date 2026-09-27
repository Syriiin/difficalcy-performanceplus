using System.Text.Json;
using Difficalcy.PerformancePlus.Models;

namespace Difficalcy.PerformancePlus.Tests;

public class OsuNonFiniteSerializationTest
{
    [Fact]
    public void DifficultyDtoWithNaNSerialises()
    {
        var dto = new OsuDifficultyAttributesDto { StarRating = double.NaN, MaxCombo = 1 };

        var json = JsonSerializer.Serialize(dto, OsuJsonContext.Default.OsuDifficultyAttributesDto);

        Assert.Contains("\"NaN\"", json);
        var deserialised = JsonSerializer.Deserialize(
            json,
            OsuJsonContext.Default.OsuDifficultyAttributesDto
        );
        Assert.True(double.IsNaN(deserialised!.StarRating));
    }

    [Fact]
    public void CalculationWithNaNSerialises()
    {
        var calculation = new OsuCalculation
        {
            Difficulty = new OsuDifficulty { Total = double.NaN },
            Performance = new OsuPerformance { Total = double.NaN },
        };

        var json = JsonSerializer.Serialize(calculation, OsuJsonContext.Default.OsuCalculation);

        Assert.Contains("\"NaN\"", json);
        var deserialised = JsonSerializer.Deserialize(json, OsuJsonContext.Default.OsuCalculation);
        Assert.True(double.IsNaN(deserialised!.Difficulty.Total));
        Assert.True(double.IsNaN(deserialised.Performance.Total));
    }
}
