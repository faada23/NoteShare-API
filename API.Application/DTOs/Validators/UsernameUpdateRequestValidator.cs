using FluentValidation;

public class UsernameUpdateRequestValidator : AbstractValidator<UsernameUpdateRequest>
{
    public UsernameUpdateRequestValidator(){
        
        RuleFor(req => req.NewUsername)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(50);
    }
}