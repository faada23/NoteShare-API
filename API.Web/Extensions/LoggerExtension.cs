using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

public static class LoggerExtension
{
    public static void AddSerilog(this IServiceCollection serviceCollection)
    {   
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            //  логи с свойством "LogType = custom"
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(logEvent =>
                    logEvent.Properties.TryGetValue("LogType", out var value) &&
                    value.ToString().Trim('"') == "custom")
                .WriteTo.File(
                    path: "Logs/custom-.log",
                    rollingInterval: RollingInterval.Hour,
                    formatter: new Serilog.Formatting.Json.JsonFormatter()
                )
            )
            // HTTP логи 
            .WriteTo.Logger(lc => lc
                .Filter.ByExcluding(logEvent =>
                    logEvent.Properties.TryGetValue("LogType", out var value) &&
                    value.ToString().Trim('"') == "custom")
                .WriteTo.File(
                    path: "Logs/HTTP-.log",
                    rollingInterval: RollingInterval.Hour,
                    formatter: new Serilog.Formatting.Json.JsonFormatter()
                )
            )
            .WriteTo.Console()
        .CreateLogger();

    }
}
