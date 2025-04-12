
public class ModeratorService : IModeratorService
{
    public IUnitOfWork UnitOfWork {get;}

    public ModeratorService(IUnitOfWork unitOfWork){
        UnitOfWork = unitOfWork;
    }

    public async Task BanUser(BanUserRequest userRequest)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == userRequest.id);
        if(user != null){
            user.IsBanned = userRequest.banStatus;

            if(user.IsBanned == true){
                var userPublicNotes = await UnitOfWork.NoteRepository
                    .GetAll(p => p.UserId == user.Id && p.IsPublic == true);

                foreach(var t in userPublicNotes){
                    UnitOfWork.NoteRepository.Delete(t);
                }
            }
            await UnitOfWork.SaveAsync();
        }
    }

    public async Task DeletePublicNote(Guid id)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);
        if(note != null){
            if(note.IsPublic == true){
                UnitOfWork.NoteRepository.Delete(note);
                await UnitOfWork.SaveAsync();
            }
        }
    }

    public Task GetLogs()
    {
        throw new NotImplementedException();
    }

}