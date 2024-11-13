using Difficalcy.Models;
using Difficalcy.PerformancePlus.Models;
using Difficalcy.PerformancePlus.Services;
using Difficalcy.Services;
using Difficalcy.Tests;
using Microsoft.Extensions.Configuration;

namespace Difficalcy.PerformancePlus.Tests;

public class OsuCalculatorServiceTest
    : CalculatorServiceTest<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>
{
    public OsuCalculatorServiceTest()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { { "OSU_COMMIT_HASH", "testhash" } }
            )
            .Build();
        CalculatorService = new OsuCalculatorService(
            new InMemoryCache(),
            new TestBeatmapProvider(typeof(OsuCalculatorService).Assembly.GetName().Name),
            configuration
        );
    }

    protected override CalculatorService<
        OsuScore,
        OsuDifficulty,
        OsuPerformance,
        OsuCalculation
    > CalculatorService { get; }

    [Theory]
    [InlineData(6.578701261037768d, 288.6125590551904d, "diffcalc-test", new string[] { })]
    [InlineData(8.8180306947868328d, 722.9095478161727d, "diffcalc-test", new string[] { "DT" })]
    public void Test(
        double expectedDifficultyTotal,
        double expectedPerformanceTotal,
        string beatmapId,
        string[] mods
    ) =>
        TestGetCalculationReturnsCorrectValues(
            expectedDifficultyTotal,
            expectedPerformanceTotal,
            new OsuScore
            {
                BeatmapId = beatmapId,
                Mods = mods.Select(m => new Mod { Acronym = m }).ToArray(),
            }
        );

    [Fact]
    public void TestAllParameters()
    {
        var score = new OsuScore
        {
            BeatmapId = "diffcalc-test",
            Mods =
            [
                new Mod() { Acronym = "HD" },
                new Mod() { Acronym = "HR" },
                new Mod() { Acronym = "DT" },
                new Mod() { Acronym = "FL" },
            ],
            Combo = 200,
            Misses = 5,
            Mehs = 4,
            Oks = 3,
        };
        TestGetCalculationReturnsCorrectValues(11.098551152482028d, 1082.5784934845988d, score);
    }
}
