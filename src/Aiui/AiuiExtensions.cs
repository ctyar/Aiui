using System.Collections.ObjectModel;
using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;

namespace Aiui;

public static class AiuiExtensions
{
    public static void AddAiui(this IServiceCollection services, AiuiOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<BotService>();
    }
}

public class AiuiOptions
{
    public OpenAIClient Client { get; set; } = null!;

    public Collection<IPlugin> Plugins = [];
}
