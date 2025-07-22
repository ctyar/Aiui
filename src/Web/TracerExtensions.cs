using OpenTelemetry.Trace;

namespace Web;

internal static class TracerExtensions
{
    public static TelemetrySpan Step(this Tracer tracer, string name, [System.Runtime.CompilerServices.CallerArgumentExpression("name")] string fullTypeName = "")
    {
        var fullName = name;
        if (fullTypeName.StartsWith("nameof(") && fullTypeName.EndsWith(")"))
        {
            fullName = fullTypeName[7..^1];
        }

        return tracer.StartActiveSpan(fullName);
    }
}
