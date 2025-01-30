using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.AspNetCore.Http;

public class NoteService : INoteService
{
    public IUnitOfWork UnitOfWork {get;}

    public NoteService(IUnitOfWork unitOfWork){

        UnitOfWork = unitOfWork;
    }

    public async Task CreateUserNote(CreateNoteRequest noteRequest, Guid userId)
    {
        var note = noteRequest.ToNote(userId);
        await UnitOfWork.NoteRepository.Insert(note);
        await UnitOfWork.SaveAsync();
    }

    public async Task DeleteUserNote(Guid id, Guid userId)
    {   
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);
        if(note.UserId == userId) await UnitOfWork.NoteRepository.Delete(id);
        await UnitOfWork.SaveAsync();
    }

    public async Task<GetNoteResponse> GetUserNote(Guid id, Guid userId)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);
        if(note.UserId != userId) return null;
        else return note.ToGetNoteResponse();
         
    }

    public async Task<IEnumerable<GetNoteResponse>> GetUserNotes(Guid userId)
    {
        var notes = await UnitOfWork.NoteRepository.GetAll();
        return notes.Where(p=> p.UserId == userId).Select(a => a.ToGetNoteResponse());
    }

    public async Task UpdateUserNote(UpdateNoteRequest noteRequest,Guid userId)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p=> p.Id == noteRequest.id);
        if(note.UserId == userId){
            note.Update(noteRequest);
            await UnitOfWork.NoteRepository.Update(note);
            await UnitOfWork.SaveAsync();
        }
    }

    public async Task NoteVisibility(Guid id,Guid userId){
        var note = await UnitOfWork.NoteRepository.GetByFilter(p=> p.Id == id);
        if(note.UserId == userId)
        {
            note.IsPublic = !note.IsPublic;
            await UnitOfWork.NoteRepository.Update(note);
            await UnitOfWork.SaveAsync();
        }
    }
}