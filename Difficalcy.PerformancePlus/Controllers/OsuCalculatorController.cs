using Difficalcy.Controllers;
using Difficalcy.PerformancePlus.Models;
using Difficalcy.PerformancePlus.Services;

namespace Difficalcy.PerformancePlus.Controllers
{
    public class OsuCalculatorController(OsuCalculatorService calculatorService)
        : CalculatorController<
            OsuScore,
            OsuDifficulty,
            OsuPerformance,
            OsuCalculation,
            OsuCalculatorService
        >(calculatorService) { }
}
