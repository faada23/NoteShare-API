using Microsoft.AspNetCore.Mvc;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        return result switch
        {
            { IsSuccess: true } => new OkObjectResult(result.Value),
            { ErrorType: ErrorType.AlreadyExists } => new ConflictObjectResult(result.Message),
            { ErrorType: ErrorType.RecordNotFound } => new NotFoundObjectResult(result.Message),
            _ => new BadRequestObjectResult(result.Message)
        };
    }
}