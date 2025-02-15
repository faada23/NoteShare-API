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

    public async Task<bool> CreateUserNote(CreateNoteRequest noteRequest, Guid userId)
    {
        var note = noteRequest.ToNote(userId);
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == userId);

        //Banned user can't share notes
        if(user.IsBanned == false || note.IsPublic == false){ 
            UnitOfWork.NoteRepository.Insert(note);
            await UnitOfWork.SaveAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteUserNote(Guid id, Guid userId)
    {   
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);

        if(note.UserId == userId){

            UnitOfWork.NoteRepository.Delete(id);
            await UnitOfWork.SaveAsync();
        }
        return false;
    }

    public async Task<GetNoteResponse?> GetUserNote(Guid id, Guid userId)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);

        if(note == null)
            return null;

        if(note.UserId != userId)
            return null;
        
        return note.ToGetNoteResponse();
         
    }

    public async Task<IEnumerable<GetNoteResponse>> GetUserNotes(Guid userId)
    {
        var notes = await UnitOfWork.NoteRepository.GetAll();
        return notes.Where(p=> p.UserId == userId).Select(a => a.ToGetNoteResponse());
    }

    public async Task<bool> UpdateUserNote(UpdateNoteRequest noteRequest,Guid userId)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p=> p.Id == noteRequest.id);
        if(note.UserId == userId){
            note.Update(noteRequest);
            UnitOfWork.NoteRepository.Update(note);
            await UnitOfWork.SaveAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> NoteVisibility(Guid id, Guid userId){
        var note = await UnitOfWork.NoteRepository.GetByFilter(p=> p.Id == id);
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == userId);

        if(note.UserId == userId && user.IsBanned == false)
        {
            note.IsPublic = !note.IsPublic;
            UnitOfWork.NoteRepository.Update(note);
            await UnitOfWork.SaveAsync();
            return true;
        }
        return false;
    }
}