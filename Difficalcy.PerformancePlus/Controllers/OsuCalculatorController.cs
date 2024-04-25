using Difficalcy.Controllers;
using Difficalcy.PerformancePlus.Models;
using Difficalcy.PerformancePlus.Services;

namespace Difficalcy.PerformancePlus.Controllers
{
    public class OsuCalculatorController : CalculatorController<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation, OsuCalculatorService>
    {
        public OsuCalculatorController(OsuCalculatorService calculatorService) : base(calculatorService) { }
    }
}
