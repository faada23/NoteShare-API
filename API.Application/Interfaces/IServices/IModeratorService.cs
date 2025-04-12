public interface IModeratorService {
    public IUnitOfWork UnitOfWork{get;}

    public Task BanUser(BanUserRequest banUserRequest);
    public Task DeletePublicNote(Guid id);
    public Task GetLogs();
}