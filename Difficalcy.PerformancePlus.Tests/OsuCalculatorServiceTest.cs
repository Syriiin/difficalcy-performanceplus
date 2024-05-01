using Difficalcy.PerformancePlus.Models;
using Difficalcy.PerformancePlus.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.PerformancePlus.Tests;

public class OsuCalculatorServiceTest : CalculatorServiceTest<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>
{
    protected override CalculatorService<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation> CalculatorService { get; } = new OsuCalculatorService(new InMemoryCache(), new TestBeatmapProvider(typeof(OsuCalculatorService).Assembly.GetName().Name));

    [Theory]
    [InlineData(6.5800462725623294d, 288.78160455080535d, "diffcalc-test", 0)]
    [InlineData(8.8116860850192751d, 721.0879910961528d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new OsuScore { BeatmapId = beatmapId, Mods = mods });

    [Fact]
    public void TestAllParameters()
    {
        var score = new OsuScore
        {
            BeatmapId = "diffcalc-test",
            Mods = 1112, // HD, HR, DT, FL
            Combo = 200,
            Misses = 5,
            Mehs = 4,
            Oks = 3,
        };
        TestGetCalculationReturnsCorrectValues(11.091002784977283, 1080.0385506471903d, score);
    }
}
