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
            // Кастомные логи (только с свойством "LogType = Custom")
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(logEvent =>
                    logEvent.Properties.TryGetValue("LogType", out var value) &&
                    value.ToString().Trim('"') == "Custom")
                .WriteTo.File(
                    path: "Logs/custom-.log",
                    rollingInterval: RollingInterval.Hour
                )
            )
            // Все остальные логи (исключая кастомные)
            .WriteTo.Logger(lc => lc
                .Filter.ByExcluding(logEvent =>
                    logEvent.Properties.TryGetValue("LogType", out var value) &&
                    value.ToString().Trim('"') == "Custom")
                .WriteTo.File(
                    path: "Logs/all-.log",
                    rollingInterval: RollingInterval.Hour
                )
            )
            .WriteTo.Console()
        .CreateLogger();

    }
}