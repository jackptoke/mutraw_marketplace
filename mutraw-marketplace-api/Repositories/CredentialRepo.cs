using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mutraw_marketplace_api.Data;
using mutraw_marketplace_api.Models;

namespace mutraw_marketplace_api.Repositories;

public class CredentialRepo(AppDbContext context) : ICredentialRepo
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public async Task<bool> AuthenticateAsync(Employee employee)
    {
        var cred = await context.Credentials.FirstOrDefaultAsync(x => x.UserName == employee.UserName);
        if(cred == null) return false;
        var result = _passwordHasher.VerifyHashedPassword(employee.UserName ?? "", cred.PasswordHash ?? "", employee.PasswordHash ?? "");
        return result == PasswordVerificationResult.Success;
    }

    public async Task CreateAsync(Employee employee)
    {
        employee.PasswordHash = _passwordHasher.HashPassword(employee.UserName ?? "", employee.PasswordHash ?? "");
        await context.Credentials.AddAsync(employee);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await context.Credentials.ToListAsync();
    }

    public async Task<Employee?> GetByUsernameAsync(string username)
    {
        return await context.Credentials.Where(x => x.UserName == username).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        employee.PasswordHash = _passwordHasher.HashPassword(employee.UserName ?? "", employee.PasswordHash ?? "");
        context.Credentials.Update(employee);
        await context.SaveChangesAsync();
    }

    public Task DeleteAsync(string username)
    {
        context.Credentials.Remove(new Employee { UserName = username });
        return context.SaveChangesAsync();
    }
}