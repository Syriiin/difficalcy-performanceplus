using System.Text.Json.Serialization;
using Difficalcy.Models;
using osu.Game.Rulesets.Osu.Difficulty;

namespace Difficalcy.PerformancePlus.Models;

// Response models
[JsonSerializable(typeof(OsuCalculation))]
[JsonSerializable(typeof(OsuCalculation[]))]
[JsonSerializable(typeof(OsuBeatmapDetails))]
// Request models
[JsonSerializable(typeof(Mod), TypeInfoPropertyName = "ScoreMod")]
[JsonSerializable(typeof(Mod[]), TypeInfoPropertyName = "ScoreModArray")]
[JsonSerializable(typeof(OsuScore))]
[JsonSerializable(typeof(OsuScore[]))]
// Internal models
[JsonSerializable(typeof(OsuDifficultyAttributes))]
[JsonSerializable(typeof(OsuDifficultyAttributesDto))]
public partial class OsuJsonContext : JsonSerializerContext { }
