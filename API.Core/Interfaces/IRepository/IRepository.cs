using System.Linq.Expressions;

public interface IRepository<T> where T : class 
{
    void Insert(T entity);
    void Delete(T entity);
    bool Delete(Guid id);
    void Update(T entity);
    Task<T> GetByFilter(Expression<Func<T, bool>> filter,string? includeProperties = null);
    Task<IEnumerable<T>> GetAll(string? includeProperties = null);
}