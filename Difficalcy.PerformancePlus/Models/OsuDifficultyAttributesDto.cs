namespace Difficalcy.PerformancePlus.Models;

public record OsuDifficultyAttributesDto
{
    public double StarRating { get; init; }
    public int MaxCombo { get; init; }
    public double AimDifficulty { get; init; }
    public double JumpAimDifficulty { get; init; }
    public double FlowAimDifficulty { get; init; }
    public double PrecisionDifficulty { get; init; }
    public double SpeedDifficulty { get; init; }
    public double StaminaDifficulty { get; init; }
    public double AccuracyDifficulty { get; init; }
    public double ApproachRate { get; init; }
    public double OverallDifficulty { get; init; }
    public int HitCircleCount { get; init; }
    public int SpinnerCount { get; init; }
}
