using System.Data;
using System.Text;
using System.Text.Json;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

public class NoteHostedCaching : BackgroundService
{   
    private const string _popularitySetKey = "notes:Popularity";
    private const string _noteCachePrefix = "note:"; 
    private const int _topN = 100;
    private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(30);
    private readonly ILogger<NoteHostedCaching> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDatabase _redisDb;
    private readonly IDistributedCache _distributedCache;
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(6);
    private readonly TimeSpan _cacheTtl = TimeSpan.FromHours(7);

    public NoteHostedCaching(
        ILogger<NoteHostedCaching> logger,
        IServiceScopeFactory scopeFactory,
        IConnectionMultiplexer connectionMultiplexer,
        IDistributedCache distributedCache)
    {   
        _logger = logger;
        _scopeFactory = scopeFactory;
        _redisDb = connectionMultiplexer.GetDatabase();
        _distributedCache = distributedCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {    
        _logger.LogInformation("Background service starting after initial delay of {InitialDelay}", _initialDelay);
        //First start delay
        await Task.Delay(_initialDelay);

        while(!stoppingToken.IsCancellationRequested){
            try{
                var getIdsResult = await GetTopNotesIds();
                if(getIdsResult.IsSuccess){
                    var cacheNotesResult = await CacheTopNotes(getIdsResult.Value!,stoppingToken);
                    if(cacheNotesResult.IsSuccess) await ResetPopularity();
                }
            }
            catch(OperationCanceledException){
                 _logger.LogInformation("Operation cancelled during processing cycle. Service stopping gracefully.");
                break;
            }
            catch(Exception ex){
                _logger.LogError(ex, "An unexpected error occurred during the processing cycle.");
            }

            try{
                await Task.Delay(_updateInterval, stoppingToken);
            }
            catch(OperationCanceledException){
                _logger.LogInformation("Operation cancelled during delay. Service stopping gracefully.");
                break;
            }    
        }
        _logger.LogInformation("Background service execution loop finished.");
    }

    private async Task<Result<List<Guid>>> GetTopNotesIds(){
        List<Guid> topNoteIdsList;

        try{
            RedisValue[] topNoteIds = await _redisDb.SortedSetRangeByRankAsync(_popularitySetKey, 0, _topN - 1, Order.Descending);
            if(topNoteIds.IsNullOrEmpty()){
                _logger.LogWarning("Redis SortedSetRangeByRankAsync returned no IDs from key {RedisKey}", _popularitySetKey);
                return Result<List<Guid>>.Failure("Error while getting top 100 popular note ids",ErrorType.RedisOperationError);
            }

            topNoteIdsList = topNoteIds
                .Select(e => Guid.TryParse(e.ToString(),out var guid) ? guid : Guid.Empty)
                .Where(g => g!= Guid.Empty)
                .ToList();
            
            if(topNoteIdsList.Count == 0) {
                 _logger.LogWarning("No valid GUIDs found after parsing {RawRedisIdCount} raw values from Redis key {RedisKey}", topNoteIds.Length, _popularitySetKey);
                return Result<List<Guid>>.Failure("No valid note ids was found after parcing",ErrorType.RedisOperationError);
            }

            return Result<List<Guid>>.Success(topNoteIdsList);
        }
        catch(Exception ex){
            _logger.LogError(ex, "Error while getting Top N notes on Redis key {RedisKey}", _popularitySetKey);
            return Result<List<Guid>>.Failure("Unknown redis error",ErrorType.RedisOperationError);
        }
    }

    private async Task<Result<int>> CacheTopNotes(List<Guid> topNoteIds,CancellationToken stoppingToken)
    {
        List<Note> notesFromDb;

        using (var scope = _scopeFactory.CreateScope()){
            
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try{
                notesFromDb = await unitOfWork.NoteRepository.GetAll(u => topNoteIds!.Contains(u.Id),includeProperties: "User");

                if(notesFromDb == null || notesFromDb.Count == 0){
                     _logger.LogWarning("No notes found in DB for the provided {NoteIdCount} IDs.", topNoteIds.Count);
                    return Result<int>.Failure("Error while getting notes from db",ErrorType.RedisOperationError);
                }

            }
            catch(Exception ex){
                _logger.LogError(ex, "Error fetching notes from database for {NoteIdCount} IDs.", topNoteIds.Count);
                return Result<int>.Failure("Unknown redis error",ErrorType.RedisOperationError);
            }
        }

        int cachedCount = 0;
        var cachedTasks = new List<Task>();
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(_cacheTtl);

        foreach(var note in notesFromDb){
            string cacheKey = $"{_noteCachePrefix}{note.Id}";

            try{
                var noteResponse = note.ToGetNoteResponse();
                string jsonNote = JsonSerializer.Serialize(noteResponse);
                byte[] dataToCache = Encoding.UTF8.GetBytes(jsonNote);

                cachedTasks.Add(_distributedCache.SetAsync(cacheKey,dataToCache,options,stoppingToken));
                cachedCount++;

            }
            catch(OperationCanceledException){
                _logger.LogInformation("Caching operation cancelled during item processing for key {CacheKey}.", cacheKey);
                throw;
            }
            catch(Exception ex){
                _logger.LogWarning(ex, "Failed to prepare note with ID {NoteId} for caching. Skipping.", note.Id);
                continue;
            }
            
        }

        try{
            await Task.WhenAll(cachedTasks);
            return Result<int>.Success(cachedCount);
        }
        catch(OperationCanceledException){
            _logger.LogInformation("Caching operation cancelled while waiting for Task.WhenAll.");
            throw;
        }
        catch(Exception ex){
            _logger.LogError(ex, "Error occurred during Task.WhenAll for caching operations. Potentially {TaskCount} tasks failed.", cachedTasks.Count);
            return Result<int>.Failure("Unknown redis error",ErrorType.RedisOperationError);
        }

    }
    
    private async Task ResetPopularity(){
        try{
            bool deleted = await _redisDb.KeyDeleteAsync(_popularitySetKey);
             if(deleted){
                 _logger.LogInformation("Successfully deleted popularity set key {RedisKey}", _popularitySetKey);
            } else {
                 _logger.LogWarning("Popularity set key {RedisKey} was not found or not deleted.", _popularitySetKey);
            }
        }
        catch(Exception ex){
            _logger.LogError(ex, "Error deleting Redis key {RedisKey}", _popularitySetKey);
        }
    }
}