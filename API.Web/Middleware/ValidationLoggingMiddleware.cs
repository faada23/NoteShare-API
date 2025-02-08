using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Serilog;
using Serilog.Context;

public class ValidationLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context){

        await _next(context);

        if (context.Request.Body != null)
        {  
            if (context.Items.TryGetValue("SanitizedDto", out var sanitizedDto))
            {   
                using (LogContext.PushProperty("LogType", "custom"))
                { 
                    Log.Information("Incoming sanitized DTO data: {@DTO}", sanitizedDto);
                }
            }
            
            if (context.Items.TryGetValue("ValidationErrors", out var validationErrors) &&
            validationErrors is List<ValidationFailure> errors)
            {
                using (LogContext.PushProperty("LogType", "custom"))
                { 
                    foreach (var error in errors)
                    {   
                        Log.Warning("Validation error for property {Property}: {ErrorMessage}",
                            error.PropertyName, error.ErrorMessage);
                    }

                }
            }

        }

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            using (LogContext.PushProperty("LogType", "custom"))
                { 
                    Log.Information("Attempted Unauthorized access by User {@Username} with id {@UserId}",
                    context.User.FindFirstValue(ClaimsIdentity.DefaultNameClaimType),
                    context.User.FindFirstValue("Id"));
                }
        }

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            using (LogContext.PushProperty("LogType", "custom"))
                { 
                    Log.Information("Attempted Unauthorized access by Anonymous");

                }
        }
    }
}

public static class ValidationLoggingExtension {
    public static IApplicationBuilder UseValidationLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ValidationLoggingMiddleware>();
    }
}


