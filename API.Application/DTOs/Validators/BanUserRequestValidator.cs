using FluentValidation;

public class BanUserRequestValidator : AbstractValidator<BanUserRequest>
{
    public BanUserRequestValidator(){

        RuleFor(req => req.Id)
            .NotNull()
            .NotEqual(Guid.Empty);
        
        RuleFor(req => req.DeletePublicNotes)
            .NotNull();
    }
}