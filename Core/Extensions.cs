using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Core;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder,
        ResourceBuilder resourceBuilder)
    {
        var otelConnectionString = builder.Configuration.GetConnectionString("OTLP_ENDPOINT_URL") ??
                                   throw new ArgumentNullException(
                                       "builder.Configuration.GetConnectionString(\"OTLP_ENDPOINT_URL\")");

        if (!string.IsNullOrWhiteSpace(otelConnectionString))
        {
            builder.Services.AddOpenTelemetry().WithMetrics(options => options.SetResourceBuilder(resourceBuilder));
            builder.Services.AddOpenTelemetry().WithTracing(options => options.SetResourceBuilder(resourceBuilder));
            builder.Logging.AddOpenTelemetry(options => options.SetResourceBuilder(resourceBuilder));
        }


        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            // http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(logging =>
        {
            if (builder.Environment.IsDevelopment()) logging.AddConsoleExporter();
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                // metrics.AddConsoleExporter();
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter()
                    .AddProcessInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    // Metrics provides by ASP.NET Core in .NET 8
                    .AddMeter(["Microsoft.AspNetCore.Hosting"])
                    .AddMeter(["Microsoft.AspNetCore.Server.Kestrel"])
                    // Metrics provided by System.Net libraries
                    .AddMeter(["System.Net.Http"])
                    .AddMeter(["System.Net.NameResolution"]);
            });

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                // tracing.AddConsoleExporter();
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddJaegerExporter()
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddMassTransitInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }


    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var otelConnectionString = builder.Configuration.GetConnectionString("OTLP_ENDPOINT_URL") ??
                                   throw new ArgumentNullException(
                                       "builder.Configuration.GetConnectionString(\"OTLP_ENDPOINT_URL\")");

        if (!string.IsNullOrWhiteSpace(otelConnectionString))
        {
            // builder.Services.AddOpenTelemetry().UseOtlpExporter();

            builder.Services.AddOpenTelemetry().WithMetrics(options => options.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(otelConnectionString);
                x.Protocol = OtlpExportProtocol.Grpc;
            }));

            builder.Services.AddOpenTelemetry().WithTracing(options => options.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(otelConnectionString);
                x.Protocol = OtlpExportProtocol.Grpc;
            }));

            builder.Logging.AddOpenTelemetry(options => options.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(otelConnectionString);
                x.Protocol = OtlpExportProtocol.Grpc;
            }));

            // builder.Services.AddOpenTelemetry().WithLogging(options => options.AddOtlpExporter(x =>
            // {
            //     x.Endpoint = new Uri(otelConnectionString);
            //     // x.Protocol = OtlpExportProtocol.Grpc;
            // }));
        }

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultLogging(this IHostApplicationBuilder builder)
    {
        var otelConnectionString = builder.Configuration.GetConnectionString("OTLP_ENDPOINT_HTTP_URL") ??
                                   throw new ArgumentNullException(
                                       "builder.Configuration.GetConnectionString(\"OTLP_ENDPOINT_HTTP_URL\")");

        if (!string.IsNullOrWhiteSpace(otelConnectionString))
        {
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext();

            loggerConfig.WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = otelConnectionString;
                options.Protocol = OtlpProtocol.HttpProtobuf;
            });

            if (builder.Environment.IsDevelopment())
            {
                loggerConfig.WriteTo.Console();
            }

            Log.Logger = loggerConfig.CreateLogger();

            builder.Logging.ClearProviders();

            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddConsole();
            }

            builder.Logging.AddSerilog(Log.Logger, dispose: true);
        }
        else if (builder.Environment.IsDevelopment())
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
        }

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}