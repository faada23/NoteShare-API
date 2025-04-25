using Microsoft.Extensions.Logging;

public class ModeratorService : IModeratorService
{
    public IUnitOfWork UnitOfWork {get;}
    private readonly ILogger<ModeratorService> _logger; 

    public ModeratorService(
        IUnitOfWork unitOfWork,
        ILogger<ModeratorService> logger) 
    {
        UnitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> SwitchBanStatus(BanUserRequest userRequest)
    {   
        _logger.LogInformation("Attempting to switch ban status for user {UserId} from moderator. DeletePublicNotes: {DeleteNotes}",
            userRequest.Id, userRequest.DeletePublicNotes);

        var user = await UnitOfWork.UserRepository.GetByFilter(
                filter: x => x.Id == userRequest.Id,
                includeProperties: "User"
            );

        foreach(var t in user.Roles)
            if(t.Name == "Moderator"){
                _logger.LogWarning("Switch ban status failed: User {UserId} is moderator.", userRequest.Id);   
                return Result<bool>.Failure("Moderator`s can`t be banned",ErrorType.InvalidInput); 
            }

        if(user == null){
            _logger.LogWarning("Switch ban status failed: User {UserId} not found.", userRequest.Id);
            return Result<bool>.Failure("User was not found",ErrorType.RecordNotFound);
        }

        user.IsBanned = !user.IsBanned;

        if(user.IsBanned && userRequest.DeletePublicNotes){
            
            _logger.LogInformation("User {UserId} notes deletion is requested.", user.Id);

            var notes = await UnitOfWork.NoteRepository.GetAll(
                filter: x => x.UserId == userRequest.Id && x.IsPublic == true
            );

            foreach(var t in notes){
                UnitOfWork.NoteRepository.Delete(t);
            }

        }

        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess){ 
            _logger.LogInformation("Successfully switched ban status for user {UserId} to {FinalBanStatus}. Notes deletion requested: {NotesDeletionRequested}, Notes deleted: {NotesDeletedCount}",
                    user.Id, user.IsBanned, userRequest.DeletePublicNotes, result.Value);
            return Result<bool>.Success(user.IsBanned);            
        }

        _logger.LogError("Failed to save ban status changes for user {UserId}. Reason: {FailureReason}",
                user.Id, result.Message);
        return Result<bool>.Failure("Error while switching ban status ",ErrorType.DatabaseError); 
    }

    public async Task<Result<Guid>> DeletePublicNote(Guid id)
    {   
        _logger.LogInformation("Attempting to delete public note {NoteId}", id);

        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);

        if(note != null && note.IsPublic == true)
        {
            UnitOfWork.NoteRepository.Delete(note);
        
            var result = await UnitOfWork.SaveAsync();

            if(result.IsSuccess) {
                _logger.LogInformation("Successfully deleted note {NoteId}.", note.Id);
                return Result<Guid>.Success(note.Id);
            }

            else{
                _logger.LogError("Failed to save deletion for note {NoteId}. Reason: {FailureReason}",
                                 note.Id, result.Message);    
                return Result<Guid>.Failure("Error while deleting note",ErrorType.DatabaseError);
            }
        }

        _logger.LogWarning("Delete public note failed: Note {NoteId} not found or not public.", id);
        return Result<Guid>.Failure("Note was not found",ErrorType.RecordNotFound);
    }
}