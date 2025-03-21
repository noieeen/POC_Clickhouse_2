using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
// Define service name and version for telemetry
var serviceName = "ServiceA";
var serviceVersion = "1.0.0";

// Configure resource for OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

// Add OpenTelemetry tracing
builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .SetResourceBuilder(resourceBuilder)
        .AddJaegerExporter()
        .AddGrpcClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
        .AddConsoleExporter() // Export logs to the console
        .AddOtlpExporter(x =>
        {
            x.Endpoint = new Uri("http://otel-collector:4317");
            x.Protocol = OtlpExportProtocol.Grpc;
        });
});

// Add OpenTelemetry metrics
builder.Services.AddOpenTelemetry().WithMetrics(meterProviderBuilder =>
{
    meterProviderBuilder
        .SetResourceBuilder(resourceBuilder)
        .AddPrometheusExporter()
        // Metrics provides by ASP.NET Core in .NET 8
        .AddMeter("Microsoft.AspNetCore.Hosting")
        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
        .AddProcessInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter(x => x.Endpoint = new Uri("http://otel-collector:4317"));
});

builder.Logging.ClearProviders();

// Configure logging
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(resourceBuilder);
    // options.AddConsoleExporter(); // Export logs to the console
    options.AddOtlpExporter(x =>
    {
        x.Endpoint = new Uri("http://otel-collector:4317");
        x.Protocol = OtlpExportProtocol.Grpc;
        // options.Headers = "";
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Sample endpoint that generates telemetry
app.MapGet("/sample", async (ILogger<Program> logger) =>
{
    var activitySource = new ActivitySource(serviceName);
    using var activity = activitySource.StartActivity("SampleOperation");

    activity?.SetTag("sample.importance", "high");

    // Log something
    logger.LogInformation("Sample endpoint called at {Time}", DateTime.UtcNow);

    // Simulate some work
    await Task.Delay(new Random().Next(10, 100));

    return "Hello from monitored .NET app!";
});
app.Run();