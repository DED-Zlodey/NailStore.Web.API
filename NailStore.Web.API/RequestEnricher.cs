using Serilog;

namespace NailStore.Web.API;

public static class RequestEnricher
{
    public static void LogAdditionalInfo(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        diagnosticContext.Set(
            "IP",
            httpContext.Connection.RemoteIpAddress?.ToString());
    }
}
