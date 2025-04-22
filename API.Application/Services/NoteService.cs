using API.Application.DTOs;
using API.Application.Mapper;
using Microsoft.Extensions.Caching.Distributed;

public class NoteService : INoteService
{   
    private const string _cachePrefix = "note:";
    private readonly INotePopularityService _notePopularityService;
    private readonly IDistributedCache _distributedCache;
    public IUnitOfWork UnitOfWork {get;}

    public NoteService(IUnitOfWork unitOfWork, INotePopularityService notePopularityService, IDistributedCache distributedCache){
        UnitOfWork = unitOfWork;
        _notePopularityService = notePopularityService;
        _distributedCache = distributedCache;
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

        if(result.IsSuccess){
            var cacheKey = $"{_cachePrefix}{note.Id}";
            try 
            {   
                await _notePopularityService.RemoveFromTopAsync(note.Id);
                await _distributedCache.RemoveAsync(cacheKey);
            }
            catch (Exception ex) {}

            return Result<Guid>.Success(note.Id);
        }
        return Result<Guid>.Failure("Error while deleting note",ErrorType.DatabaseError);

    }

    public async Task<Result<GetNoteResponse>> GetUserNote(Guid id, Guid userId)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(
            filter: p => p.Id == id,
            includeProperties: "User");

        if(note == null || note.UserId != userId) return Result<GetNoteResponse>.Failure("Note was not found",ErrorType.RecordNotFound);

        return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
    }

    public async Task<Result<PagedResponse<GetNotePreviewResponse>>> GetUserPreviewNotes(Guid userId,PaginationParameters? pagParams)
    {   
        var notes = await UnitOfWork.NoteRepository.GetAll(
            filter: p=> p.UserId == userId,
            pagParams: pagParams,
            includeProperties: "User");

        var pagedResponse = Mapper.ToPagedResponse(notes);

        return Result<PagedResponse<GetNotePreviewResponse>>.Success(pagedResponse);
    }

    public async Task<Result<GetNoteResponse>> UpdateUserNote(UpdateNoteRequest noteRequest,Guid userId)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(
            filter: p=> p.Id == noteRequest.Id);

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

        if(result.IsSuccess) {
            if(note.IsPublic == false){
                var cacheKey = $"{_cachePrefix}{note.Id}";
                try 
                {   
                    await _notePopularityService.RemoveFromTopAsync(note.Id);
                    await _distributedCache.RemoveAsync(cacheKey);
                }
                catch (Exception ex) {}
            }
            return Result<bool>.Success(note.IsPublic);
        }
        return Result<bool>.Failure("Error while changing note visibility",ErrorType.DatabaseError);

        
    }

}