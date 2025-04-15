using System.Data;
using API.Application.DTOs;
using FluentValidation;

public class UpdateNoteRequestValidator : AbstractValidator<UpdateNoteRequest> 
{
    public UpdateNoteRequestValidator(){
        
        RuleFor(note => note.Id)
            .NotNull()
            .NotEqual(Guid.Empty);
            
        RuleFor(note => note.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(note => note.Content)
            .NotNull();
    }
}