using API.Application.DTOs;
using FluentValidation;

public class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator(){

        RuleFor(note => 
            note.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(note => 
            note.Content)
            .NotNull();  
    }
}