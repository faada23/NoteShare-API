using API.Application.DTOs;
using FluentValidation;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(){

        RuleFor(req => req.Username)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(req => req.Password)
            .NotEmpty()
            .MaximumLength(50);

    }
}