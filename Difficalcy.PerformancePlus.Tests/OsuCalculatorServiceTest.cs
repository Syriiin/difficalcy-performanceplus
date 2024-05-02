using Difficalcy.PerformancePlus.Models;
using Difficalcy.PerformancePlus.Services;
using Difficalcy.Services;
using Difficalcy.Tests;
using Microsoft.Extensions.Configuration;

namespace Difficalcy.PerformancePlus.Tests;

public class OsuCalculatorServiceTest : CalculatorServiceTest<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>
{
    public OsuCalculatorServiceTest()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
            {"OSU_COMMIT_HASH", "testhash"},
        }).Build();
        CalculatorService = new OsuCalculatorService(new InMemoryCache(), new TestBeatmapProvider(typeof(OsuCalculatorService).Assembly.GetName().Name), configuration);
    }

    protected override CalculatorService<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation> CalculatorService { get; }

    [Theory]
    [InlineData(6.578701261037768d, 288.6125590551904d, "diffcalc-test", 0)]
    [InlineData(8.8180306947868328d, 722.9095478161727d, "diffcalc-test", 64)]
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
        TestGetCalculationReturnsCorrectValues(11.098551152482028d, 1082.5784934845988d, score);
    }
}
