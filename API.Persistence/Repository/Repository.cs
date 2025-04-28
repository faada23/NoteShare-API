using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : class
{
    public DatabaseContext _db {get; }
    public DbSet<T> dbSet;

    public Repository(DatabaseContext db){
        _db = db;
        dbSet = _db.Set<T>();
    }

    public void Insert(T entity)
    {
        dbSet.Add(entity);
    }

    public void Delete(T entity)
    {
        if (dbSet.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
    }


    public async Task<PagedList<T>> GetAll(
        Expression<Func<T, bool>>? filter = null,
        PaginationParameters? pagParams = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
        string? includeProperties = null
        )
    {
        IQueryable<T> query = dbSet;

        if(filter != null)
        {
            query = query.Where(filter);
        }

        //ef.property translates to sql and works on db side
        orderBy ??= q => q.OrderByDescending(e => EF.Property<Guid>(e,"Id"));
        
        var totalItems = await query.CountAsync();

        if(pagParams != null){
            
            query = query
                .Skip((pagParams.Page - 1) * pagParams.PageSize)
                .Take(pagParams.PageSize);

            query = IncludeProperties(query, includeProperties);

            var pagedList = await query.ToListAsync();
            return PagedList<T>.ToPagedList(pagedList,pagParams.Page,pagParams.PageSize,totalItems);
        }

        query = IncludeProperties(query, includeProperties);

        var list = await query.ToListAsync();
        return PagedList<T>.ToPagedList(list,1,totalItems,totalItems);
    }

    public async Task<T> GetByFilter(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);

        query = IncludeProperties(query, includeProperties);

        return await query.FirstOrDefaultAsync();
    }


    public void Update(T entity)
    {
        dbSet.Update(entity);
    }
    
    private IQueryable<T> IncludeProperties(IQueryable<T> query, string? includeProperties)
    {   
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return query;
    }
}