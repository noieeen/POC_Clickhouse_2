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

        builder.Services.ConfigureHttpClientDefaults(http => { http.AddServiceDiscovery(); });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(logging =>
        {
            if (builder.Environment.IsDevelopment())
                logging.AddConsoleExporter();

            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddPrometheusExporter()
                    .AddProcessInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(["Microsoft.AspNetCore.Hosting"])
                    .AddMeter(["Microsoft.AspNetCore.Server.Kestrel"])
                    .AddMeter(["System.Net.Http"])
                    .AddMeter(["System.Net.NameResolution"])
                    // RabbitMQ
                    .AddMeter(["RabbitMQ.Metrics"]);
            });

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = httpContext =>
                            !httpContext.Request.Path.StartsWithSegments("/metrics");
                    })
                    .AddJaegerExporter()
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

            // Optional OTLP log export (currently commented out)
            // builder.Logging.AddOpenTelemetry(options => options.AddOtlpExporter(x =>
            // {
            //     x.Endpoint = new Uri(otelConnectionString);
            //     x.Protocol = OtlpExportProtocol.Grpc;
            // }));
        }

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
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
                .Filter.ByExcluding("RequestPath like '/health%'")
                .Filter.ByExcluding("RequestPath like '/metrics%'")
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
        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecks("/health");

            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}