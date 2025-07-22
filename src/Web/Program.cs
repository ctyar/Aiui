using Aiui;
using Microsoft.Extensions.AI;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Web.Components;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            AddLogging(builder);

            builder.AddServiceDefaults();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.AddAzureOpenAIClient("openai")
                .AddChatClient("gpt-4o-mini")
                .UseFunctionInvocation()
                .UseOpenTelemetry(configure: c =>
                    c.EnableSensitiveData = builder.Environment.IsDevelopment());

            builder.Services.AddSingleton(new SqlListPlugin(builder.Configuration.GetConnectionString("SqlServer")!,
            [
                "Products",
#if !DEBUG
            "Categories",
            "CustomerCustomerDemo",
            "CustomerDemographics",
            "Customers",
            "Employees",
            "EmployeeTerritories",
            "Order Details",
            "Orders",
            "Region",
            "Shippers",
            "Suppliers",
            "Territories"
#endif
        ]));

            builder.Services.AddSingleton<ChartJsPlugin>();

            var app = builder.Build();

            app.MapDefaultEndpoints();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAntiforgery();

            app.UseStaticFiles();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

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
        loggerConfiguration.Enrich.WithProperty("Version", Version.Current);

        loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

        loggerConfiguration.WriteTo.Console(theme: AnsiConsoleTheme.Code);

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.Services.AddSerilog();
    }
}
