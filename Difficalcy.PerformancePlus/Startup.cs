using System.Reflection;
using Difficalcy.PerformancePlus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Difficalcy.PerformancePlus
{
    public class Startup : DifficalcyStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration) { }

        public override string OpenApiTitle => "Difficalcy.PerformancePlus";

        public override string OpenApiVersion => "v1";

        protected override string TestBeatmapAssembly =>
            Assembly.GetExecutingAssembly().GetName().Name;

        public override void ConfigureCalculatorServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(OsuCalculatorService));
        }
    }
}
