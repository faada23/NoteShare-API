using FluentValidation;

public class UsernameUpdateRequestValidator : AbstractValidator<UsernameUpdateRequest>
{
    public UsernameUpdateRequestValidator(){
        
        RuleFor(req => req.newUsername)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(50);
    }
}