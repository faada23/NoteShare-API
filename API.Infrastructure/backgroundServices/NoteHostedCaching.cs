using System.Data;
using System.Text;
using System.Text.Json;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

public class NoteHostedCaching : BackgroundService
{   
     private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDatabase _redisDb;
    private readonly IDistributedCache _distributedCache;
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(6);
    private readonly TimeSpan _cacheTtl = TimeSpan.FromHours(7);
    private const string _popularitySetKey = "notes:Popularity";
    private const string _noteCachePrefix = "note:"; 
    private const int _topN = 100;

    public NoteHostedCaching(
        IServiceScopeFactory scopeFactory,
        IConnectionMultiplexer connectionMultiplexer,
        IDistributedCache distributedCache)
    {
        _scopeFactory = scopeFactory;
        _redisDb = connectionMultiplexer.GetDatabase();
        _distributedCache = distributedCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {       
        //First start delay
        await Task.Delay(TimeSpan.FromSeconds(30));

        while(!stoppingToken.IsCancellationRequested){
            try{
                var getIdsResult = await GetTopNotesIds();
                if(getIdsResult.IsSuccess){
                    var cacheNotesResult = await CacheTopNotes(getIdsResult.Value!,stoppingToken);
                    if(cacheNotesResult.IsSuccess) await ResetPopularity();
                }
            }
            catch(OperationCanceledException){
                break;
            }
            catch(Exception){
                
            }

            try{
                await Task.Delay(_updateInterval, stoppingToken);
            }
            catch(OperationCanceledException){
                break;
            }    
        }
    }

    private async Task<Result<List<Guid>>> GetTopNotesIds(){
        List<Guid> topNoteIdsList;

        try{
            RedisValue[] topNoteIds = await _redisDb.SortedSetRangeByRankAsync(_popularitySetKey, 0, _topN - 1, Order.Descending);
            if(topNoteIds.IsNullOrEmpty()){
                return Result<List<Guid>>.Failure("Error while getting top 100 popular note ids",ErrorType.RedisOperationError);
            }

            topNoteIdsList = topNoteIds
                .Select(e => Guid.TryParse(e.ToString(),out var guid) ? guid : Guid.Empty)
                .Where(g => g!= Guid.Empty)
                .ToList();
            
            if(topNoteIdsList.Count == 0) {
                return Result<List<Guid>>.Failure("No valid note ids was found after parcing",ErrorType.RedisOperationError);
            }

            return Result<List<Guid>>.Success(topNoteIdsList);
        }
        catch(Exception ex){
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
                    return Result<int>.Failure("Error while getting notes from db",ErrorType.RedisOperationError);
                }

            }
            catch(Exception ex){
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
            catch(Exception){
                continue;
            }
            
        }

        try{
            await Task.WhenAll(cachedTasks);
            return Result<int>.Success(cachedCount);
        }
        catch(OperationCanceledException){
            throw;
        }
        catch(Exception){
            return Result<int>.Failure("Unknown redis error",ErrorType.RedisOperationError);
        }

    }
    
    private async Task ResetPopularity(){
        try{
            bool deleted = await _redisDb.KeyDeleteAsync(_popularitySetKey);
        }
        catch(Exception){
            return;
        }
    }
}