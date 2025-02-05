public interface IModeratorService {
    public IUnitOfWork UnitOfWork{get;}

    public Task<bool> BanUser(BanUserRequest banUserRequest);
    public Task<bool> DeletePublicNote(Guid id);
    public Task GetLogs();
}