using API.Core.Models;

public interface INotePopularityService {
    public Task<Result<int>> IncrementPopularityAsync(Guid noteId);
    public Task<Result<bool>> IsNoteInTopAsync(Guid noteId, int topN);
    public Task<Result<bool>> RemoveFromTopAsync(Guid noteId);
}