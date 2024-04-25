using Difficalcy.Models;

namespace Difficalcy.PerformancePlus.Models
{
    public record OsuDifficulty : Difficulty
    {
        public double Aim { get; init; }
        public double JumpAim { get; init; }
        public double FlowAim { get; init; }
        public double Precision { get; init; }
        public double Speed { get; init; }
        public double Stamina { get; init; }
        public double Accuracy { get; init; }
    }
}
