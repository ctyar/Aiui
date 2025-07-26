using Aiui;
using Microsoft.Extensions.AI;
using Web.Components;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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
}
