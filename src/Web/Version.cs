using System.Reflection;

namespace Web;

internal static class Version
{
    private static string? CurrentValue;

    public static string Current
    {
        get
        {
            if (CurrentValue is null)
            {
                CurrentValue = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(3) ?? string.Empty;
            }

            return CurrentValue;
        }
    }
}
