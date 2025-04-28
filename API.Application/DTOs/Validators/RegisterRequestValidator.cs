using API.Application.DTOs;
using FluentValidation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(){

        RuleFor(req => req.Username)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(50);

        RuleFor(req => req.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(50)
            .Matches(@"\d")
            .WithMessage("'Password' must contain numbers")
            .NotEqual(req => req.Username)
            .WithMessage("'Password' can't match with 'Username'");
    }
}