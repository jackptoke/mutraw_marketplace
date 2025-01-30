using mutraw_marketplace_api.Models;

namespace mutraw_marketplace_api.Repositories;

public interface ICredentialRepo
{
    Task<bool> AuthenticateAsync(Credential credential);
    Task CreateAsync(Credential credential);
    Task<IEnumerable<Credential>> GetAllAsync();
    Task UpdateAsync(Credential credential);
    Task DeleteAsync(string username);
}