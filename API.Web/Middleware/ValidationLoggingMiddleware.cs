using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Serilog;
using Serilog.Context;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context){

        await _next(context);
   
    }
}

public static class LoggingExtension {
    public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoggingMiddleware>();
    }
}


