using API.Application.DTOs;
using API.Application.Mapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

public class NoteService : INoteService
{   
    private const string _cachePrefix = "note:";
    private readonly INotePopularityService _notePopularityService;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<NoteService> _logger;
    public IUnitOfWork UnitOfWork {get;}

    public NoteService(IUnitOfWork unitOfWork, INotePopularityService notePopularityService, IDistributedCache distributedCache, ILogger<NoteService> logger)
    {
        UnitOfWork = unitOfWork;
        _notePopularityService = notePopularityService;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<Result<GetNoteResponse>> CreateUserNote(CreateNoteRequest noteRequest, Guid userId)
    {   
        _logger.LogInformation("Attempting to create note for User {UserId}", userId);

        var note = noteRequest.ToNote(userId);
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == userId);

        if(user == null){
            _logger.LogWarning("Create note failed: User {UserId} not found.", userId);
            return Result<GetNoteResponse>.Failure("User was not found",ErrorType.RecordNotFound);
        }

        //Banned user can't share notes
        if(user.IsBanned && note.IsPublic){ 
            _logger.LogWarning("Create note failed: Banned User {UserId} tried to create a public note.", userId);
            return Result<GetNoteResponse>.Failure("Current user can`t create public notes",ErrorType.UserIsBanned);
        }

        UnitOfWork.NoteRepository.Insert(note);
        var result = await UnitOfWork.SaveAsync();
        if(result.IsSuccess){
            _logger.LogInformation("Successfully created Note {NoteId} for User {UserId}", note.Id, userId);
            return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
        }

        _logger.LogError("Failed to save new note for User {UserId}. Reason: {FailureReason}", userId, result.Message);
        return Result<GetNoteResponse>.Failure("Error while inserting note",ErrorType.DatabaseError);
        
    }

    public async Task<Result<Guid>> DeleteUserNote(Guid id, Guid userId)
    {   
        _logger.LogInformation("Attempting to delete Note {NoteId} by User {UserId}", id, userId);

        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);

        if(note == null || note.UserId != userId){
            _logger.LogWarning("Delete note failed: Note {NoteId} not found or does not belong to User {UserId}.", id, userId);
            return Result<Guid>.Failure("Note was not found",ErrorType.RecordNotFound);
        }

        UnitOfWork.NoteRepository.Delete(note);
        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess){
            _logger.LogInformation("Successfully deleted Note {NoteId} by User {UserId}", note.Id, userId);
            var cacheKey = $"{_cachePrefix}{note.Id}";
            try 
            {   
                await _notePopularityService.RemoveFromTopAsync(note.Id);
                _logger.LogInformation("Removed Note {NoteId} from popularity service.", note.Id);
                await _distributedCache.RemoveAsync(cacheKey);
                _logger.LogInformation("Removed Note {NoteId} from cache (Key: {CacheKey}).", note.Id, cacheKey);
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Failed to remove Note {NoteId} from popularity service or cache after deletion.", note.Id);
            }

            return Result<Guid>.Success(note.Id);
        }

        _logger.LogError("Failed to save deletion for Note {NoteId}. Reason: {FailureReason}", id, result.Message);
        return Result<Guid>.Failure("Error while deleting note",ErrorType.DatabaseError);

    }

    public async Task<Result<GetNoteResponse>> GetUserNote(Guid id, Guid userId)
    {   
        _logger.LogInformation("Attempting to get Note {NoteId} for User {UserId}", id, userId);
        var note = await UnitOfWork.NoteRepository.GetByFilter(
            filter: p => p.Id == id,
            includeProperties: "User");

        if(note == null || note.UserId != userId){
            _logger.LogWarning("Get note failed: Note {NoteId} not found or does not belong to User {UserId}.", id, userId);    
            return Result<GetNoteResponse>.Failure("Note was not found",ErrorType.RecordNotFound);
        }

        _logger.LogInformation("Successfully retrieved Note {NoteId} for User {UserId}", id, userId);
        return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
    }

    public async Task<Result<PagedResponse<GetNotePreviewResponse>>> GetUserPreviewNotes(Guid userId,PaginationParameters? pagParams)
    {   
        _logger.LogInformation("Attempting to get notes preview for User {UserId}. PageNumber: {PageNumber}, PageSize: {PageSize}",
            userId, pagParams?.Page, pagParams?.PageSize);

        var notes = await UnitOfWork.NoteRepository.GetAll(
            filter: p=> p.UserId == userId,
            pagParams: pagParams,
            includeProperties: "User");

        var pagedResponse = Mapper.ToPagedResponse(notes);

        _logger.LogInformation("Successfully retrieved {NoteCount} notes preview for User {UserId}. PageNumber: {PageNumber}, PageSize: {PageSize}",
            pagedResponse.Data.Count(), userId, pagParams?.Page, pagParams?.PageSize);
        return Result<PagedResponse<GetNotePreviewResponse>>.Success(pagedResponse);
    }

    public async Task<Result<GetNoteResponse>> UpdateUserNote(UpdateNoteRequest noteRequest,Guid userId)
    {   
        _logger.LogInformation("Attempting to update Note {NoteId} by User {UserId}", noteRequest.Id, userId);

        var note = await UnitOfWork.NoteRepository.GetByFilter(
            filter: p=> p.Id == noteRequest.Id);

        if(note == null || note.UserId != userId){
            _logger.LogWarning("Update note failed: Note {NoteId} not found or does not belong to User {UserId}.", noteRequest.Id, userId);
            return Result<GetNoteResponse>.Failure("Note was not found",ErrorType.RecordNotFound);
        }

        note.Update(noteRequest);
        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess){
            _logger.LogInformation("Successfully updated Note {NoteId} by User {UserId}", note.Id, userId);
            return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
        }
    
        _logger.LogError("Failed to save update for Note {NoteId}. Reason: {FailureReason}", noteRequest.Id, result.Message);   
        return Result<GetNoteResponse>.Failure("Error while updating note",ErrorType.DatabaseError);
        
    }

    public async Task<Result<bool>> NoteVisibility(Guid id, Guid userId){
        _logger.LogInformation("Attempting to toggle visibility for Note {NoteId} by User {UserId}", id, userId);
        var note = await UnitOfWork.NoteRepository.GetByFilter(p=> p.Id == id,"User");

        if(note == null || note.UserId != userId){
            _logger.LogWarning("Toggle visibility failed: Note {NoteId} not found or does not belong to User {UserId}.", id, userId);
            return Result<bool>.Failure("Note was not found",ErrorType.RecordNotFound);
        }

        note.IsPublic = !note.IsPublic;
        
        //banned user can`t share notes(make them public)
        if(note.User!.IsBanned && note.IsPublic){
            _logger.LogWarning("Toggle visibility failed: Banned User {UserId} tried to make Note {NoteId} public.", userId, id);
            return Result<bool>.Failure("Current user can`t share notes",ErrorType.UserIsBanned);
        }

        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess) {
            if(note.IsPublic == false){
                var cacheKey = $"{_cachePrefix}{note.Id}";
                try 
                {   
                    await _notePopularityService.RemoveFromTopAsync(note.Id);
                    _logger.LogInformation("Removed Note {NoteId} from popularity service due to becoming private.", note.Id);
                    await _distributedCache.RemoveAsync(cacheKey);
                    _logger.LogInformation("Removed Note {NoteId} from cache due to becoming private (Key: {CacheKey}).", note.Id, cacheKey);
                }
                catch (Exception ex) {
                    _logger.LogWarning(ex, "Failed to remove Note {NoteId} from popularity service or cache after making private.", note.Id);
                }
            }
            _logger.LogInformation("Successfully changed Note {NoteId} visibility to {IsPublic}", note.Id, note.IsPublic);
            return Result<bool>.Success(note.IsPublic);
        }
        
        _logger.LogError("Failed to save visibility change for Note {NoteId}. Reason: {FailureReason}", id, result.Message);
        return Result<bool>.Failure("Error while changing note visibility",ErrorType.DatabaseError);

        
    }

}