using System.Text;
using System.Text.Json;
using API.Application.DTOs;
using API.Application.Mapper;
using Microsoft.Extensions.Caching.Distributed;

public class SharedService : ISharedService
{
    private const string _noteCachePrefix  = "note:";
    private const int topN = 100;
    private readonly IDistributedCache _distributedCache;
    private readonly INotePopularityService _notePopularityService;

    public IUnitOfWork UnitOfWork {get;}
    
    public SharedService(IUnitOfWork unitOfWork, IDistributedCache distributedCache, INotePopularityService notePopularityService){

        UnitOfWork = unitOfWork;
        _distributedCache = distributedCache;
        _notePopularityService = notePopularityService;
    }

    //Todo Logging
    public async Task<Result<GetNoteResponse>> GetSharedNote(Guid id)
    {   
        Result<bool> result = await _notePopularityService.IsNoteInTopAsync(id,topN);

        if(result.Value) 
            try{
                string cacheKey = $"{_noteCachePrefix}{id}";
                byte[]? cachedData = await _distributedCache.GetAsync(cacheKey);

                if(cachedData != null) {
                    var cachedNote = JsonSerializer.Deserialize<GetNoteResponse>(Encoding.UTF8.GetString(cachedData));

                    if(cachedNote != null){
                        await _notePopularityService.IncrementPopularityAsync(id);
                        return Result<GetNoteResponse>.Success(cachedNote);
                    }

                    await _distributedCache.RemoveAsync(id.ToString());
                    await _notePopularityService.RemoveFromTopAsync(id);
                }
            }
            catch(Exception ex){

            }

        var note = await UnitOfWork.NoteRepository.GetByFilter(
            filter: p => p.Id == id && p.IsPublic,
            includeProperties: "User");

        if(note == null) return Result<GetNoteResponse>.Failure("Note was not found",ErrorType.RecordNotFound);

        await _notePopularityService.IncrementPopularityAsync(id);
        return Result<GetNoteResponse>.Success(note.ToGetNoteResponse());
    }

    public async Task<Result<PagedResponse<GetNotePreviewResponse>>> GetSharedPreviewNotes(PaginationParameters? pagParams)
    {
        var notes = await UnitOfWork.NoteRepository.GetAll(
            filter: p=> p.IsPublic,
            pagParams: pagParams,
            includeProperties: "User"
            );

        var pagedResponse = Mapper.ToPagedResponse(notes);

        return Result<PagedResponse<GetNotePreviewResponse>>.Success(pagedResponse);
    }
}