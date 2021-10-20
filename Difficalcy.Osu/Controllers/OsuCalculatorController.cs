using Difficalcy.Controllers;
using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;

namespace Difficalcy.Osu.Controllers
{
    public class OsuCalculatorController : CalculatorController<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation, OsuCalculatorService>
    {
        public OsuCalculatorController(OsuCalculatorService calculatorService) : base(calculatorService) { }
    }
}
