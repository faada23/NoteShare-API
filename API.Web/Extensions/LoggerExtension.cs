using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

public static class LoggerExtension
{
    public static void AddSerilog(this IServiceCollection serviceCollection)
    {   
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            //  HTTP логи
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(IsHttpLog)
                .WriteTo.File(
                    path: "Logs/HTTP-.log",
                    rollingInterval: RollingInterval.Day,
                    formatter: new Serilog.Formatting.Json.JsonFormatter()
                )
            )
            // Все логи кроме HTTP 
            .WriteTo.Logger(lc => lc
                .Filter.ByExcluding(IsHttpLog)
                .WriteTo.File(
                    path: "Logs/ALL-.log",
                    rollingInterval: RollingInterval.Day,
                    formatter: new Serilog.Formatting.Json.JsonFormatter()
                )
            )
            .WriteTo.Console()
        .CreateLogger();

    }

    private static bool IsHttpLog(LogEvent logEvent) =>
        logEvent.Properties.TryGetValue("LogType", out var logTypeValue) &&
        logTypeValue is ScalarValue scalarValue && 
        scalarValue.Value?.ToString() == "http"; 
    
}
