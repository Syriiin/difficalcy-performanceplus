using Difficalcy.Models;

namespace Difficalcy.Osu.Models
{
    public record OsuScore : Score
    {
        public double? Accuracy { get; init; }
        public int? Combo { get; init; }
        public int? Misses { get; init; }
        public int? Mehs { get; init; }
        public int? Oks { get; init; }
    }
}
