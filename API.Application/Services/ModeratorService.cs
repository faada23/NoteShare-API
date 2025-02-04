
public class ModeratorService : IModeratorService
{
    public IUnitOfWork UnitOfWork {get;}

    public ModeratorService(IUnitOfWork unitOfWork){
        UnitOfWork = unitOfWork;
    }

    public async Task BanUser(BanUserRequest userRequest)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == userRequest.id);
        user.IsBanned = userRequest.banStatus;
        await UnitOfWork.SaveAsync();
    }

    public async Task DeletePublicNote(Guid id)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);
        if(note.IsPublic == true){
            await UnitOfWork.NoteRepository.Delete(note);
            await UnitOfWork.SaveAsync();
        }
    }

    public Task GetLogs()
    {
        throw new NotImplementedException();
    }

}