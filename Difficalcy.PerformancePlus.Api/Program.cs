using System.Collections.Generic;
using Difficalcy;
using Difficalcy.Models;
using Difficalcy.PerformancePlus.Models;
using Difficalcy.PerformancePlus.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddDifficalcyServices("Difficalcy.PerformancePlus", "v1");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Add(DifficalcyJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(OsuJsonContext.Default);
});

builder.Services.AddSingleton<OsuCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

var handlers = new DifficalcyHandlers<
    OsuScore,
    OsuDifficulty,
    OsuPerformance,
    OsuCalculation,
    OsuBeatmapDetails
>(app.Services.GetRequiredService<OsuCalculatorService>());

var api = app.MapGroup("/api");

api.MapGet("/calculators", () =>
{
    var osu = app.Services.GetRequiredService<OsuCalculatorService>();
    return new Dictionary<string, CalculatorInfo>
    {
        ["osu"] = osu.Info,
    };
});

var osu = api.MapGroup("/calculators/osu");
osu.MapGet("/info", handlers.GetInfo);
osu.MapGet("/calculation", handlers.GetCalculation);
osu.MapPost("/batch/calculation", handlers.GetCalculationBatch);
osu.MapGet("/beatmapdetails", handlers.GetBeatmapDetails);

app.Run();
