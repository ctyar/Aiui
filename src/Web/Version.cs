using System.Reflection;

namespace AiuiWeb;

internal static class Version
{
    private static string? CurrentValue;

    public static string Current
    {
        get
        {
            if (CurrentValue is null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                CurrentValue = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;
            }

            return CurrentValue;
        }
    }
}
