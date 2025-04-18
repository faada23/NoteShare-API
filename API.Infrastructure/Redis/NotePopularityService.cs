using StackExchange.Redis;

public class NotePopularityService : INotePopularityService
{
    private readonly IDatabase _database;
    private const string _popularitySetKey  = "notes:Popularity";
    public NotePopularityService(
        IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public Task<Result<int>> IncrementPopularityAsync(Guid noteId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> IsNoteInTopAsync(Guid noteId, int topN)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFromTopAsync()
    {
        throw new NotImplementedException();
    }
}