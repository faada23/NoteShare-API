
public class ModeratorService : IModeratorService
{
    public IUnitOfWork UnitOfWork {get;}

    public ModeratorService(IUnitOfWork unitOfWork){
        UnitOfWork = unitOfWork;
    }

    public async Task<bool> BanUser(BanUserRequest userRequest)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == userRequest.id);
        if(user != null){
            user.IsBanned = userRequest.banStatus;
            await UnitOfWork.SaveAsync();
            return true;
        }
        else return false;
    }

    public async Task<bool> DeletePublicNote(Guid id)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);
        if(note != null){
            if(note.IsPublic == true){
                UnitOfWork.NoteRepository.Delete(note);
                await UnitOfWork.SaveAsync();
                return true;
            }
        }
        return false;
    }

    public Task GetLogs()
    {
        throw new NotImplementedException();
    }

}