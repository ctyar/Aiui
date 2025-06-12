using System.Diagnostics.Metrics;
using Aiui;
using AiuiWeb.RequestLog;
using Azure.AI.OpenAI;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace AiuiWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AddLogging(builder);

        AddTraceAndMetrics(builder);

        try
        {
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            builder.Services.AddAiui(new AiuiOptions
            {
                Client = new OpenAIClient(builder.Configuration.GetValue<string>("OpenApiKey")),
                Plugins =
                [
                    new SqlListPlugin(builder.Configuration.GetConnectionString("SqlServer")!, ["Categories",
                        "CustomerCustomerDemo",
                        "CustomerDemographics",
                        "Customers",
                        "Employees",
                        "EmployeeTerritories",
                        "Order Details",
                        "Orders",
                        "Products",
                        "Region",
                        "Shippers",
                        "Suppliers",
                        "Territories"]),
                    new ChartJsPlugin()
                ]
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<RequestLogService>();
            builder.Services.AddSingleton<ListService>();
            builder.Services.AddSingleton<GraphService>();
            builder.Services.AddSingleton<DataStorageService>();

            var app = builder.Build();

            app.UseExceptionHandler("/error");

            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void AddLogging(WebApplicationBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.Enrich.FromLogContext();
        loggerConfiguration.Enrich.WithProperty("commitHash", Version.Current);

        loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

        if (builder.Environment.IsDevelopment())
        {
            loggerConfiguration.WriteTo.Console(theme: AnsiConsoleTheme.Code);
        }
        else
        {
            builder.WebHost.UseSentry();

            loggerConfiguration.WriteTo.Console(theme: AnsiConsoleTheme.Code);

            loggerConfiguration.WriteTo.Sentry(o =>
            {
                o.MinimumEventLevel = LogEventLevel.Warning;
            });

            builder.WebHost.UseSentry();
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.Host.UseSerilog();
    }

    private static void AddTraceAndMetrics(WebApplicationBuilder builder)
    {
        var honeycombOptions = builder.Configuration.GetHoneycombOptions();

        builder.Services.AddOpenTelemetry()
            .WithTracing(b =>
            {
                // TODO: Enable or remove this
                b//.AddHoneycomb(honeycombOptions)
                    .AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

        builder.Services.AddOpenTelemetry().WithMetrics(b =>
            {
                // TODO: Enable or remove this
                b//.AddHoneycomb(honeycombOptions)
                    .AddAspNetCoreInstrumentation();
            });

        builder.Services.AddSingleton(new Meter(honeycombOptions.MetricsDataset));
    }
}
