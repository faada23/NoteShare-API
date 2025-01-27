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

    public async Task Insert(T entity)
    {
        await dbSet.AddAsync(entity);
    }

    public Task Delete(T entity)
    {
        if (dbSet.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);

        return Task.CompletedTask;
    }

    public Task Delete(Guid id)
    {
        T entity = dbSet.Find(id);
        dbSet.Remove(entity);

        return Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> GetAll(string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return await query.ToListAsync();
    }

    public async Task<T> GetByFilter( Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);
        if(!string.IsNullOrEmpty(includeProperties)){
            foreach(var includeProp in includeProperties
            .Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)){
                query = query.Include(includeProp);
            }
        }
        return await query.FirstOrDefaultAsync();
    }


    public Task Update(T entity)
    {
        dbSet.Update(entity);
        return Task.CompletedTask;
    }
    
}