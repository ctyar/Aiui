using Microsoft.Extensions.DependencyInjection;

namespace Aiui;

public static class AiuiExtensions
{
    public static void AddAiui(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddSingleton<BotService>();
        services.AddSingleton<SqlServerService>();
    }
}
