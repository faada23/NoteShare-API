using StackExchange.Redis;

public class NotePopularityService : INotePopularityService
{
    private const string _popularitySetKey  = "notes:Popularity";
    private readonly IDatabase _database;

    public NotePopularityService(
        IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<Result<int>> IncrementPopularityAsync(Guid noteId)
    {
        if(noteId == Guid.Empty) return Result<int>.Failure("",ErrorType.RecordNotFound);

        var noteIdStr = noteId.ToString();

        try{
            double newScore = await _database.SortedSetIncrementAsync(_popularitySetKey,noteIdStr,1.0);
            return Result<int>.Success((int)newScore);
        }
        catch(Exception ex){
            return Result<int>.Failure(ex.Message,ErrorType.RedisOperationError);
        }
    }

    public async Task<Result<bool>> IsNoteInTopAsync(Guid noteId, int topN)
    {
        if(noteId == Guid.Empty) return Result<bool>.Failure("",ErrorType.RecordNotFound);

        if(topN <= 0)  return Result<bool>.Failure("",ErrorType.InvalidInput);

        var noteIdStr = noteId.ToString();

        try{
            long? rank = await _database.SortedSetRankAsync(_popularitySetKey,noteIdStr,Order.Descending);
            
            if(rank <= topN)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Success(false);
        }
        catch(Exception ex){
            return Result<bool>.Failure(ex.Message,ErrorType.RedisOperationError);
        }
    }

    public async Task<Result<bool>> RemoveFromTopAsync(Guid noteId)
    {   
        if(noteId == Guid.Empty) return Result<bool>.Failure("",ErrorType.RecordNotFound);
        
        var noteIdStr = noteId.ToString();

        try{
            var result = await _database.SortedSetRemoveAsync(_popularitySetKey,noteIdStr);
            return Result<bool>.Success(result);
        }
        catch(Exception ex){
            return Result<bool>.Failure(ex.Message,ErrorType.RedisOperationError);
        }
    }
}