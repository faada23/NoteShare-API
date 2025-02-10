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

    public bool Delete(Guid id)
    {   
        T? entity = dbSet.Find(id);

        if(entity != null){

            dbSet.Remove(entity);
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null,string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        
        if(filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return await query.ToListAsync();
    }

    public async Task<T> GetByFilter(Expression<Func<T, bool>> filter, string? includeProperties = null)
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


    public void Update(T entity)
    {
        dbSet.Update(entity);
    }
    
}