using Difficalcy.Models;
using Difficalcy.PerformancePlus.Models;
using Difficalcy.PerformancePlus.Services;
using Difficalcy.Services;
using Microsoft.Extensions.Configuration;

namespace Difficalcy.PerformancePlus.Tests;

public class OsuCalculatorServiceTest
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

    private CalculatorService<
        OsuScore,
        OsuDifficulty,
        OsuPerformance,
        OsuCalculation,
        OsuBeatmapDetails
    > CalculatorService { get; }

    [Fact]
    public async Task Test()
    {
        var score = new OsuScore { BeatmapId = "diffcalc-test", Mods = [] };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(6.578701261037768d, calculation.Difficulty.Total, 4);
        Assert.Equal(288.6125590551904d, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);
        Assert.Equal(239, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestWithDT()
    {
        var score = new OsuScore
        {
            BeatmapId = "diffcalc-test",
            Mods = [new Mod() { Acronym = "DT" }],
        };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(8.8180306947868328d, calculation.Difficulty.Total, 4);
        Assert.Equal(722.9095478161727d, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);
        Assert.Equal(239, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParameters()
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

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(11.098551152482028d, calculation.Difficulty.Total, 4);
        Assert.Equal(1082.5784934845988d, calculation.Performance.Total, 4);
        Assert.Equal(0.91666666666666663, calculation.Accuracy, 4);
        Assert.Equal(200, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
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
