using System.Linq.Expressions;

public interface IRepository<T> where T : class 
{
    void Insert(T entity);
    void Delete(T entity);
    void Delete(int id);
    void Update(T entity);
    T GetByFilter(Expression<Func<T, bool>> filter,string? includeProperties = null);
    IEnumerable<T> GetAll(string? includeProperties = null);
}