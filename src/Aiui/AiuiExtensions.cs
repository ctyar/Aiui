using Microsoft.Extensions.DependencyInjection;

namespace Aiui;

public static class AiuiExtensions
{
    public static void AddAiui(this IServiceCollection services)
    {
        services.AddSingleton<BotService>();
    }
}
