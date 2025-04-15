
public class ModeratorService : IModeratorService
{
    public IUnitOfWork UnitOfWork {get;}

    public ModeratorService(IUnitOfWork unitOfWork){
        UnitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> SwitchBanStatus(BanUserRequest userRequest)
    {   
        var user = await UnitOfWork.UserRepository.GetByFilter(
                filter: x => x.Id == userRequest.Id
            );
        if(user == null) return Result<bool>.Failure("User was not found",ErrorType.RecordNotFound);

        user.IsBanned = !user.IsBanned;

        if(user.IsBanned && userRequest.DeletePublicNotes){

            var notes = await UnitOfWork.NoteRepository.GetAll(
                filter: x => x.UserId == userRequest.Id && x.IsPublic == true
            );

            foreach(var t in notes){
                UnitOfWork.NoteRepository.Delete(t);
            }

        }

        var result = await UnitOfWork.SaveAsync();
        if(result.IsSuccess) return Result<bool>.Success(user.IsBanned);
        return Result<bool>.Failure("Error while deleting notes",ErrorType.DatabaseError); 
    }

    public async Task<Result<Guid>> DeletePublicNote(Guid id)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);
        if(note != null && note.IsPublic == true)
        {
            UnitOfWork.NoteRepository.Delete(note);
        
            var result = await UnitOfWork.SaveAsync();
            if(result.IsSuccess) return Result<Guid>.Success(note.Id);
            else return Result<Guid>.Failure("Error while deleting note",ErrorType.DatabaseError);
        }
        return Result<Guid>.Failure("Note was not found",ErrorType.RecordNotFound);
    }

    public Task GetLogs()
    {
        throw new NotImplementedException();
    }

}