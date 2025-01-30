namespace mutraw_marketplace_api.Repositories;

public interface IRepository<T>
{
    Task<T?> GetIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(long id);
    Task<T?> GetOneByCustomSqlAsync(string query);
}