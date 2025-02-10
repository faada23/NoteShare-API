using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;


public class LoggingValidatorInterceptor : IValidatorInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggingValidatorInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public FluentValidation.Results.ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, FluentValidation.Results.ValidationResult result)
    {   
        var dto = validationContext.InstanceToValidate;
        var sanitizedDto = RemovePasswordProperty(dto);
        
        if(_httpContextAccessor.HttpContext != null){
            _httpContextAccessor.HttpContext.Items["SanitizedDto"] = sanitizedDto;
        }

        if (!result.IsValid)
        {   
            if (_httpContextAccessor.HttpContext != null){
            // Сохраняем ошибки валидации в HttpContext.Items
            _httpContextAccessor.HttpContext.Items["ValidationErrors"] = result.Errors;
            }
        }
        
        return result;
    }

    public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
    {
        return commonContext;
    }

    private object RemovePasswordProperty(object dto)
    {
        var properties = dto.GetType().GetProperties();
        var sanitizedDto = new Dictionary<string, object>();

        foreach (var property in properties)
        {
            if (!property.Name.Equals("Password", StringComparison.OrdinalIgnoreCase))
            {
                sanitizedDto[property.Name] = property.GetValue(dto);
            }
        }

        return sanitizedDto;
    }
}