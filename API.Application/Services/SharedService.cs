using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;

public class SharedService : ISharedService
{
    public IUnitOfWork UnitOfWork {get;}

    public SharedService(IUnitOfWork unitOfWork){

        UnitOfWork = unitOfWork;
    }

    public async Task<Result<GetNoteResponse>> GetSharedNote(Guid id)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id && p.IsPublic);

        if(note == null) return Result<GetNoteResponse>.Failure("Note was not found",ErrorType.RecordNotFound);

        return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
    }

    public async Task<Result<PagedResponse<GetNotePreviewResponse>>> GetSharedPreviewNotes(PaginationParameters? pagParams)
    {
        var notes = await UnitOfWork.NoteRepository.GetAll(
            filter: p=> p.IsPublic,
            pagParams: pagParams
            );

        var pagedResponse = Mapper.ToPagedResponse(notes);

        return Result<PagedResponse<GetNotePreviewResponse>>.Success(pagedResponse);
    }
}