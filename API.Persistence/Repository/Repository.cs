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

    public virtual void Delete(T entity)
    {
         if (dbSet.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
    }

    public void Delete(int id)
    {
        T entity = dbSet.Find(id);
        dbSet.Remove(entity);
    }

    public IEnumerable<T> GetAll(string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return query.ToList();
    }

    public T GetByFilter(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);
        if(!string.IsNullOrEmpty(includeProperties)){
            foreach(var includeProp in includeProperties
            .Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)){
                query = query.Include(includeProp);
            }
        }
        return query.FirstOrDefault();
    }


    public void Update(T entity)
    {
        dbSet.Update(entity);
    }
    
}