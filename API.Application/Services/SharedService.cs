using System.Text;
using System.Text.Json;
using API.Application.DTOs;
using API.Application.Mapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

public class SharedService : ISharedService
{
    private const string _noteCachePrefix  = "note:";
    private const int topN = 100;
    private readonly IDistributedCache _distributedCache;
    private readonly INotePopularityService _notePopularityService;
    private readonly ILogger<SharedService> _logger;
    public IUnitOfWork UnitOfWork {get;}

    public SharedService(IUnitOfWork unitOfWork, IDistributedCache distributedCache, INotePopularityService notePopularityService, ILogger<SharedService> logger)
    {
        UnitOfWork = unitOfWork;
        _distributedCache = distributedCache;
        _notePopularityService = notePopularityService;
        _logger = logger;
    }

    public async Task<Result<GetNoteResponse>> GetSharedNote(Guid id)
    {   
        _logger.LogInformation("Attempting to get shared Note {NoteId}", id);

        if(id == Guid.Empty){
            _logger.LogWarning("Note {NoteId} is null", id);  
            return Result<GetNoteResponse>.Failure("Note id is null",ErrorType.InvalidInput);
        }

        string cacheKey = $"{_noteCachePrefix}{id}";
        try{
            _logger.LogInformation("Note {NoteId}, attempting cache retrieval.", id);
            
            byte[]? cachedData = await _distributedCache.GetAsync(cacheKey);

            if(cachedData != null) {
                var cachedNote = JsonSerializer.Deserialize<GetNoteResponse>(Encoding.UTF8.GetString(cachedData));

                if(cachedNote != null){
                    _logger.LogInformation("Cache hit successful for Note {NoteId}. Incrementing popularity.", id);
                    await _notePopularityService.IncrementPopularityAsync(id);
                    return Result<GetNoteResponse>.Success(cachedNote);
                }

                _logger.LogWarning("Cache data for Note {NoteId} deserialized to null. Removing invalid cache entry.", id);
                await _distributedCache.RemoveAsync(cacheKey);
                await _notePopularityService.RemoveFromTopAsync(id);
            }

            _logger.LogInformation("Cache miss for  Note {NoteId}.", id);
        }
        catch(Exception ex){
            _logger.LogError(ex, "Error accessing or processing cache for Note {NoteId}. CacheKey: {CacheKey}", id, cacheKey);
        }

        _logger.LogInformation("Atempting database lookup for public Note {NoteId}", id);
        var note = await UnitOfWork.NoteRepository.GetByFilter(
            filter: p => p.Id == id && p.IsPublic,
            includeProperties: "User");

        if(note == null){
            _logger.LogWarning("Get shared note failed: Public Note {NoteId} not found in database.", id);
            return Result<GetNoteResponse>.Failure("Note was not found",ErrorType.RecordNotFound);
        }
        
        await _notePopularityService.IncrementPopularityAsync(id);
        _logger.LogInformation("Successfully retrieved public Note {NoteId} from database. Incrementing popularity.", id);

        return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
    }

    public async Task<Result<PagedResponse<GetNotePreviewResponse>>> GetSharedPreviewNotes(PaginationParameters? pagParams)
    {   
        _logger.LogInformation("Attempting to get shared notes preview. PageNumber: {PageNumber}, PageSize: {PageSize}",
            pagParams?.Page, pagParams?.PageSize);

        var notes = await UnitOfWork.NoteRepository.GetAll(
            filter: p=> p.IsPublic,
            pagParams: pagParams,
            includeProperties: "User"
            );

        var pagedResponse = Mapper.ToPagedResponse(notes);

        _logger.LogInformation("Successfully retrieved shared notes preview. PageNumber: {PageNumber}, PageSize: {PageSize}",
            pagParams?.Page, pagParams?.PageSize);
        return Result<PagedResponse<GetNotePreviewResponse>>.Success(pagedResponse);
    }
}