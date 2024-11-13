using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Difficalcy.Models;

namespace Difficalcy.PerformancePlus.Models
{
    public record OsuScore : Score
    {
        [Range(0, int.MaxValue)]
        public int? Combo { get; init; }

        [Range(0, int.MaxValue)]
        public int Misses { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Mehs { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Oks { get; init; } = 0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Misses > 0 && Combo is null)
            {
                yield return new ValidationResult(
                    "Combo must be specified if Misses are greater than 0.",
                    [nameof(Combo)]
                );
            }
        }
    }
}
