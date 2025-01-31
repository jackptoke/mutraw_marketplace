using mutraw_marketplace_api.Models;

namespace mutraw_marketplace_api.Repositories;

public interface ICredentialRepo
{
    Task<bool> AuthenticateAsync(Employee employee);
    Task CreateAsync(Employee employee);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByUsernameAsync(string username);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(string username);
}