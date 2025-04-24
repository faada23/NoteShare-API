using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

public class NotePopularityService : INotePopularityService
{
    private const string _popularitySetKey  = "notes:Popularity";
    private readonly IDatabase _database;
    private readonly ILogger<NotePopularityService> _logger; 

    public NotePopularityService(
        ILogger<NotePopularityService> logger,
        IConnectionMultiplexer connectionMultiplexer)
    {   
        _logger = logger;
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<Result<int>> IncrementPopularityAsync(Guid noteId)
    {
        if(noteId == Guid.Empty){
             _logger.LogWarning("IncrementPopularityAsync called with empty Guid.");
            return Result<int>.Failure("NoteId cannot be empty.", ErrorType.InvalidInput);
        }

        var noteIdStr = noteId.ToString();

        try{
            double newScore = await _database.SortedSetIncrementAsync(_popularitySetKey,noteIdStr,1.0);
            return Result<int>.Success((int)newScore);
        }
        catch(Exception ex){
            _logger.LogError(ex, "Redis error incrementing popularity for NoteId {NoteId} in key {RedisKey}", noteId, _popularitySetKey);
            return Result<int>.Failure(ex.Message,ErrorType.RedisOperationError);
        }
    }

    public async Task<Result<bool>> IsNoteInTopAsync(Guid noteId, int topN)
    {
        if(noteId == Guid.Empty){
            _logger.LogWarning("IsNoteInTopAsync called with empty Guid.");
            return Result<bool>.Failure("NoteId cannot be empty.", ErrorType.InvalidInput);
        }

        if(topN <= 0){
            _logger.LogWarning("IsNoteInTopAsync called with invalid TopN value: {TopN}", topN);
            return Result<bool>.Failure("",ErrorType.InvalidInput);
        }

        var noteIdStr = noteId.ToString();

        try{
            long? rank = await _database.SortedSetRankAsync(_popularitySetKey,noteIdStr,Order.Descending);
            
            if(rank <= topN)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Success(false);
        }
        catch(Exception ex){
            _logger.LogError(ex, "Redis error checking rank for NoteId {NoteId} in key {RedisKey}", noteId, _popularitySetKey);
            return Result<bool>.Failure(ex.Message,ErrorType.RedisOperationError);
        }
    }

    public async Task<Result<bool>> RemoveFromTopAsync(Guid noteId)
    {   
        if(noteId == Guid.Empty)
        {
             _logger.LogWarning("RemoveFromTopAsync called with empty Guid.");
            return Result<bool>.Failure("NoteId cannot be empty.", ErrorType.InvalidInput);
        }
        
        var noteIdStr = noteId.ToString();

        try{
            var result = await _database.SortedSetRemoveAsync(_popularitySetKey,noteIdStr);
             if (!result) 
                _logger.LogInformation("NoteId {NoteId} was not found in popularity set {RedisKey} for removal", noteId, _popularitySetKey);

            return Result<bool>.Success(result);
        }
        catch(Exception ex){
            _logger.LogError(ex, "Redis error removing NoteId {NoteId} from key {RedisKey}", noteId, _popularitySetKey);
            return Result<bool>.Failure(ex.Message,ErrorType.RedisOperationError);
        }
    }
}