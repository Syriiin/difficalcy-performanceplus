using Difficalcy.Models;

namespace Difficalcy.Osu.Models
{
    public record OsuPerformance : Performance
    {
        public double Aim { get; init; }
        public double Speed { get; init; }
        public double Accuracy { get; init; }
        public double Flashlight { get; init; }
    }
}
