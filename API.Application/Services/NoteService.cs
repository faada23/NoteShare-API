using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;

public class NoteService : INoteService
{
    public IUnitOfWork UnitOfWork {get;}

    public NoteService(IUnitOfWork unitOfWork){

        UnitOfWork = unitOfWork;
    }

    public async Task<Result<GetNoteResponse>> CreateUserNote(CreateNoteRequest noteRequest, Guid userId)
    {
        var note = noteRequest.ToNote(userId);
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == userId);

        if(user == null) return Result<GetNoteResponse>.Failure("User was not found",ErrorType.RecordNotFound);

        //Banned user can't share notes
        if(user.IsBanned && note.IsPublic){ 
            return Result<GetNoteResponse>.Failure("Current user can`t create public notes",ErrorType.UserIsBanned);
        }

        UnitOfWork.NoteRepository.Insert(note);
        var result = await UnitOfWork.SaveAsync();
        if(result.IsSuccess) return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
        else return Result<GetNoteResponse>.Failure("Error while inserting note",ErrorType.DatabaseError);
    }

    public async Task<Result<Guid>> DeleteUserNote(Guid id, Guid userId)
    {   
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);

        if(note == null || note.UserId != userId){
            return Result<Guid>.Failure("Note was not found",ErrorType.RecordNotFound);
        }

        UnitOfWork.NoteRepository.Delete(note);
        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess) return Result<Guid>.Success(note.Id);
        return Result<Guid>.Failure("Error while Deleting note",ErrorType.DatabaseError);

    }

    public async Task<Result<GetNoteResponse>> GetUserNote(Guid id, Guid userId)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);

        if(note == null || note.UserId != userId) return Result<GetNoteResponse>.Failure("Note was not found",ErrorType.RecordNotFound);

        return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
    }

    public async Task<Result<PagedResponse<GetNoteResponse>>> GetUserNotes(Guid userId,PaginationParameters pagParams)
    {   
        var notes = await UnitOfWork.NoteRepository.GetAll(
            filter: p=> p.UserId == userId,
            pagParams: pagParams
            );

        var pagedResponse = Mapper.ToPagedResponse(notes);

        return Result<PagedResponse<GetNoteResponse>>.Success(pagedResponse);
    }

    public async Task<Result<GetNoteResponse>> UpdateUserNote(UpdateNoteRequest noteRequest,Guid userId)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p=> p.Id == noteRequest.id);

        if(note == null || note.UserId != userId) return Result<GetNoteResponse>.Failure("Note was not found",ErrorType.RecordNotFound);

        note.Update(noteRequest);
        var result = await UnitOfWork.SaveAsync();
        if(result.IsSuccess) return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
        
        else return Result<GetNoteResponse>.Failure("Errow while updating note",ErrorType.DatabaseError);

    }

    public async Task<Result<bool>> NoteVisibility(Guid id, Guid userId){
        var note = await UnitOfWork.NoteRepository.GetByFilter(p=> p.Id == id,"User");

        if(note == null || note.UserId != userId) return Result<bool>.Failure("Note was not found",ErrorType.RecordNotFound);

        //banned user can`t share notes(make them public)
        if(note.User!.IsBanned && note.IsPublic) return Result<bool>.Failure("Current user can`t share notes",ErrorType.UserIsBanned);

        note.IsPublic = !note.IsPublic;

        var result = await UnitOfWork.SaveAsync();
        if(result.IsSuccess) return Result<bool>.Success(note.IsPublic);
        return Result<bool>.Failure("Error while changing note visibility",ErrorType.DatabaseError);

        
    }

}