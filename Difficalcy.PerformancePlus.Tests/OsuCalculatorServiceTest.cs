using Difficalcy.Models;
using Difficalcy.PerformancePlus.Models;
using Difficalcy.PerformancePlus.Services;
using Difficalcy.Services;
using Difficalcy.Tests;
using Microsoft.Extensions.Configuration;

namespace Difficalcy.PerformancePlus.Tests;

public class OsuCalculatorServiceTest
    : CalculatorServiceTest<
        OsuScore,
        OsuDifficulty,
        OsuPerformance,
        OsuCalculation,
        OsuBeatmapDetails
    >
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
        OsuCalculation,
        OsuBeatmapDetails
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

    [Fact]
    public async Task TestGetBeatmapDetails()
    {
        var beatmapId = "diffcalc-test";
        var beatmapDetails = await CalculatorService.GetBeatmapDetails(beatmapId);
        Assert.Equal("Unknown", beatmapDetails.Artist);
        Assert.Equal("Unknown", beatmapDetails.Title);
        Assert.Equal("Normal", beatmapDetails.DifficultyName);
        Assert.Equal("Unknown Creator", beatmapDetails.Author);
        Assert.Equal(239, beatmapDetails.MaxCombo);
        Assert.Equal(102500, beatmapDetails.Length);
        Assert.Equal(120, beatmapDetails.MinBPM);
        Assert.Equal(120, beatmapDetails.MaxBPM);
        Assert.Equal(120, beatmapDetails.CommonBPM);
        Assert.Equal(79, beatmapDetails.CircleCount);
        Assert.Equal(33, beatmapDetails.SliderCount);
        Assert.Equal(12, beatmapDetails.SpinnerCount);
        Assert.Equal(82, beatmapDetails.SliderTickCount);
        Assert.Equal(4, beatmapDetails.CircleSize);
        Assert.Equal(8.3, beatmapDetails.ApproachRate, 4);
        Assert.Equal(7, beatmapDetails.Accuracy);
        Assert.Equal(5, beatmapDetails.DrainRate);
        Assert.Equal(1.6, beatmapDetails.BaseVelocity, 4);
        Assert.Equal(1, beatmapDetails.TickRate);
    }
}
