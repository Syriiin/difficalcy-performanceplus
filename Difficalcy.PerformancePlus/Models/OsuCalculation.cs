using Difficalcy.Models;

namespace Difficalcy.PerformancePlus.Models
{
    public record OsuCalculation : Calculation<OsuDifficulty, OsuPerformance>
    {
        public double Accuracy { get; init; }
        public double Combo { get; init; }
    }
}
