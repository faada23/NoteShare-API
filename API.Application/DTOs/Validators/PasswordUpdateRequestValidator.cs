using FluentValidation;

public class PasswordUpdateRequestValidator : AbstractValidator<PasswordUpdateRequest>
{
    public PasswordUpdateRequestValidator(){
        
         RuleFor(req => req.newPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(50)
            .Matches(@"\d")
            .WithMessage("'Password' must contain numbers");
    }
}