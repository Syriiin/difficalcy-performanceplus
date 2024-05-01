using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.PerformancePlus.Models;
using Difficalcy.Services;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace Difficalcy.PerformancePlus.Services
{
    public class OsuCalculatorService(ICache cache, IBeatmapProvider beatmapProvider) : CalculatorService<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>(cache)
    {
        private OsuRuleset OsuRuleset { get; } = new OsuRuleset();

        public override CalculatorInfo Info
        {
            get
            {
                var packageName = "https://github.com/Syriiin/osu";
                var packageVersion = "f32875ab59108ece030f6bd8373a6368636356c2";
                return new CalculatorInfo
                {
                    RulesetName = OsuRuleset.Description,
                    CalculatorName = "PerformancePlus (PP+)",
                    CalculatorPackage = packageName,
                    CalculatorVersion = packageVersion,
                    CalculatorUrl = $"{packageName}/tree/{packageVersion}"
                };
            }
        }

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(OsuScore score)
        {
            var workingBeatmap = GetWorkingBeatmap(score.BeatmapId);
            var mods = OsuRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();

            var difficultyCalculator = OsuRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as OsuDifficultyAttributes;

            // Serialising anonymous object with same names because Mods and Skills can't be serialised
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                difficultyAttributes.StarRating,
                difficultyAttributes.MaxCombo,
                difficultyAttributes.AimDifficulty,
                difficultyAttributes.JumpAimDifficulty,
                difficultyAttributes.FlowAimDifficulty,
                difficultyAttributes.PrecisionDifficulty,
                difficultyAttributes.SpeedDifficulty,
                difficultyAttributes.StaminaDifficulty,
                difficultyAttributes.AccuracyDifficulty,
                difficultyAttributes.ApproachRate,
                difficultyAttributes.OverallDifficulty,
                difficultyAttributes.HitCircleCount,
                difficultyAttributes.SpinnerCount
            }));
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<OsuDifficultyAttributes>(difficultyAttributesJson, new JsonSerializerOptions() { IncludeFields = true });
        }

        protected override OsuCalculation CalculatePerformance(OsuScore score, object difficultyAttributes)
        {
            var osuDifficultyAttributes = (OsuDifficultyAttributes)difficultyAttributes;

            var workingBeatmap = GetWorkingBeatmap(score.BeatmapId);
            var mods = OsuRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(OsuRuleset.RulesetInfo, mods);

            var combo = score.Combo ?? beatmap.HitObjects.Count + beatmap.HitObjects.OfType<Slider>().Sum(s => s.NestedHitObjects.Count - 1);
            var statistics = GetHitResults(beatmap.HitObjects.Count, score.Misses, score.Mehs, score.Oks);
            var accuracy = CalculateAccuracy(statistics);

            var scoreInfo = new ScoreInfo()
            {
                Accuracy = accuracy,
                MaxCombo = combo,
                Statistics = statistics,
                Mods = mods
            };

            var performanceCalculator = OsuRuleset.CreatePerformanceCalculator();
            var performanceAttributes = performanceCalculator.Calculate(scoreInfo, osuDifficultyAttributes) as OsuPerformanceAttributes;

            return new OsuCalculation()
            {
                Difficulty = GetDifficultyFromDifficultyAttributes(osuDifficultyAttributes),
                Performance = GetPerformanceFromPerformanceAttributes(performanceAttributes),
                Accuracy = accuracy,
                Combo = combo
            };
        }

        private CalculatorWorkingBeatmap GetWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(OsuRuleset, beatmapStream, beatmapId);
        }

        private static Dictionary<HitResult, int> GetHitResults(int hitResultCount, int countMiss, int countMeh, int countOk)
        {
            var countGreat = hitResultCount - countOk - countMeh - countMiss;

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countGreat },
                { HitResult.Ok, countOk },
                { HitResult.Meh, countMeh },
                { HitResult.Miss, countMiss }
            };
        }

        private static double CalculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            var countGreat = statistics[HitResult.Great];
            var countOk = statistics[HitResult.Ok];
            var countMeh = statistics[HitResult.Meh];
            var countMiss = statistics[HitResult.Miss];
            var total = countGreat + countOk + countMeh + countMiss;

            return (double)((6 * countGreat) + (2 * countOk) + countMeh) / (6 * total);
        }

        private static OsuDifficulty GetDifficultyFromDifficultyAttributes(OsuDifficultyAttributes difficultyAttributes)
        {
            return new OsuDifficulty()
            {
                Total = difficultyAttributes.StarRating,
                Aim = difficultyAttributes.AimDifficulty,
                JumpAim = difficultyAttributes.JumpAimDifficulty,
                FlowAim = difficultyAttributes.FlowAimDifficulty,
                Precision = difficultyAttributes.PrecisionDifficulty,
                Speed = difficultyAttributes.SpeedDifficulty,
                Stamina = difficultyAttributes.StaminaDifficulty,
                Accuracy = difficultyAttributes.AccuracyDifficulty
            };
        }

        private static OsuPerformance GetPerformanceFromPerformanceAttributes(OsuPerformanceAttributes performanceAttributes)
        {
            return new OsuPerformance()
            {
                Total = performanceAttributes.Total,
                Aim = performanceAttributes.Aim,
                JumpAim = performanceAttributes.JumpAim,
                FlowAim = performanceAttributes.FlowAim,
                Precision = performanceAttributes.Precision,
                Speed = performanceAttributes.Speed,
                Stamina = performanceAttributes.Stamina,
                Accuracy = performanceAttributes.Accuracy
            };
        }
    }
}
