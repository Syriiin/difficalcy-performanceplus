using Difficalcy.Models;

namespace Difficalcy.Osu.Models
{
    public record OsuDifficulty : Difficulty
    {
        public double Aim { get; init; }
        public double Speed { get; init; }
        public double Flashlight { get; init; }
    }
}
