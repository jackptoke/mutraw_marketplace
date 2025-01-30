using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mutraw_marketplace_api.Data;
using mutraw_marketplace_api.Models;

namespace mutraw_marketplace_api.Repositories;

public class CredentialRepo(AppDbContext context) : ICredentialRepo
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public async Task<bool> AuthenticateAsync(Credential credential)
    {
        var cred = await context.Credentials.FirstOrDefaultAsync(x => x.Username == credential.Username);
        if(cred == null) return false;
        var result = _passwordHasher.VerifyHashedPassword(credential.Username, cred.Password, credential.Password);
        return result == PasswordVerificationResult.Success;
    }

    public async Task CreateAsync(Credential credential)
    {
        credential.Password = _passwordHasher.HashPassword(credential.Username, credential.Password);
        await context.Credentials.AddAsync(credential);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Credential>> GetAllAsync()
    {
        return await context.Credentials.ToListAsync();
    }

    public async Task UpdateAsync(Credential credential)
    {
        credential.Password = _passwordHasher.HashPassword(credential.Username, credential.Password);
        context.Credentials.Update(credential);
        await context.SaveChangesAsync();
    }

    public Task DeleteAsync(string username)
    {
        context.Credentials.Remove(new Credential { Username = username });
        return context.SaveChangesAsync();
    }
}