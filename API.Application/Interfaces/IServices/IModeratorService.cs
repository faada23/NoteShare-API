public interface IModeratorService {
    public IUnitOfWork UnitOfWork{get;}

    public Task<Result<bool>> SwitchBanStatus(BanUserRequest banUserRequest);
    public Task<Result<Guid>> DeletePublicNote(Guid id);
    
}