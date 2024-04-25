using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.PerformancePlus.Models;
using Difficalcy.Services;
using Microsoft.Extensions.Configuration;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using StackExchange.Redis;

namespace Difficalcy.PerformancePlus.Services
{
    public class OsuCalculatorService : CalculatorService<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>
    {
        private readonly IBeatmapProvider _beatmapProvider;
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

        public OsuCalculatorService(ICache cache, IBeatmapProvider beatmapProvider) : base(cache)
        {
            _beatmapProvider = beatmapProvider;
        }

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await _beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(OsuScore score)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = OsuRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();

            var difficultyCalculator = OsuRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as OsuDifficultyAttributes;

            // Serialising anonymous object with same names because Mods and Skills can't be serialised
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                StarRating = difficultyAttributes.StarRating,
                MaxCombo = difficultyAttributes.MaxCombo,
                AimStrain = difficultyAttributes.AimStrain,
                JumpAimStrain = difficultyAttributes.JumpAimStrain,
                FlowAimStrain = difficultyAttributes.FlowAimStrain,
                PrecisionStrain = difficultyAttributes.PrecisionStrain,
                SpeedStrain = difficultyAttributes.SpeedStrain,
                StaminaStrain = difficultyAttributes.StaminaStrain,
                AccuracyStrain = difficultyAttributes.AccuracyStrain,
                ApproachRate = difficultyAttributes.ApproachRate,
                OverallDifficulty = difficultyAttributes.OverallDifficulty,
                HitCircleCount = difficultyAttributes.HitCircleCount,
                SpinnerCount = difficultyAttributes.SpinnerCount
            }));
        }

        protected override OsuDifficulty GetDifficultyFromDifficultyAttributes(object difficultyAttributes)
        {
            var osuDifficultyAttributes = (OsuDifficultyAttributes)difficultyAttributes;
            return new OsuDifficulty()
            {
                Total = osuDifficultyAttributes.StarRating,
                Aim = osuDifficultyAttributes.AimStrain,
                JumpAim = osuDifficultyAttributes.JumpAimStrain,
                FlowAim = osuDifficultyAttributes.FlowAimStrain,
                Precision = osuDifficultyAttributes.PrecisionStrain,
                Speed = osuDifficultyAttributes.SpeedStrain,
                Stamina = osuDifficultyAttributes.StaminaStrain,
                Accuracy = osuDifficultyAttributes.AccuracyStrain
            };
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<OsuDifficultyAttributes>(difficultyAttributesJson, new JsonSerializerOptions() { IncludeFields = true });
        }

        protected override OsuPerformance CalculatePerformance(OsuScore score, object difficultyAttributes)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = OsuRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(OsuRuleset.RulesetInfo, mods);

            var combo = score.Combo ?? beatmap.HitObjects.Count + beatmap.HitObjects.OfType<Slider>().Sum(s => s.NestedHitObjects.Count - 1);
            var statistics = determineHitResults(score.Accuracy ?? 1, beatmap.HitObjects.Count, score.Misses ?? 0, score.Mehs, score.Oks);
            var accuracy = calculateAccuracy(statistics);

            var scoreInfo = new ScoreInfo()
            {
                Accuracy = accuracy,
                MaxCombo = combo,
                Statistics = statistics,
                Mods = mods
            };

            var performanceCalculator = OsuRuleset.CreatePerformanceCalculator((OsuDifficultyAttributes)difficultyAttributes, scoreInfo);
            var categoryAttributes = new Dictionary<string, double>();
            var performance = performanceCalculator.Calculate(categoryAttributes);

            return new OsuPerformance()
            {
                Total = performance,
                Aim = categoryAttributes["Aim"],
                JumpAim = categoryAttributes["Jump Aim"],
                FlowAim = categoryAttributes["Flow Aim"],
                Precision = categoryAttributes["Precision"],
                Speed = categoryAttributes["Speed"],
                Stamina = categoryAttributes["Stamina"],
                Accuracy = categoryAttributes["Accuracy"]
            };
        }

        protected override OsuCalculation GetCalculation(OsuDifficulty difficulty, OsuPerformance performance)
        {
            return new OsuCalculation()
            {
                Difficulty = difficulty,
                Performance = performance
            };
        }

        private CalculatorWorkingBeatmap getWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = _beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(OsuRuleset, beatmapStream, beatmapId);
        }

        private Dictionary<HitResult, int> determineHitResults(double targetAccuracy, int hitObjectCount, int countMiss, int? countMeh, int? countOk)
        {
            // Adapted from https://github.com/ppy/osu-tools/blob/cf5410b04f4e2d1ed2c50c7263f98c8fc5f928ab/PerformanceCalculator/Simulate/OsuSimulateCommand.cs#L57-L91
            int countGreat;

            if (countMeh != null || countOk != null)
            {
                countGreat = hitObjectCount - (countOk ?? 0) - (countMeh ?? 0) - countMiss;
            }
            else
            {
                // Let Great=6, Ok=2, Meh=1, Miss=0. The total should be this.
                var targetTotal = (int)Math.Round(targetAccuracy * hitObjectCount * 6);

                // Start by assuming every non miss is a meh
                // This is how much increase is needed by greats and oks
                var delta = targetTotal - (hitObjectCount - countMiss);

                // Each great increases total by 5 (great-meh=5)
                countGreat = delta / 5;
                // Each ok increases total by 1 (ok-meh=1). Covers remaining difference.
                countOk = delta % 5;
                // Mehs are left over. Could be negative if impossible value of amountMiss chosen
                countMeh = hitObjectCount - countGreat - countOk - countMiss;
            }

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countGreat },
                { HitResult.Ok, countOk ?? 0 },
                { HitResult.Meh, countMeh ?? 0 },
                { HitResult.Miss, countMiss }
            };
        }

        private double calculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            var countGreat = statistics[HitResult.Great];
            var countOk = statistics[HitResult.Ok];
            var countMeh = statistics[HitResult.Meh];
            var countMiss = statistics[HitResult.Miss];
            var total = countGreat + countOk + countMeh + countMiss;

            return (double)((6 * countGreat) + (2 * countOk) + countMeh) / (6 * total);
        }
    }
}
