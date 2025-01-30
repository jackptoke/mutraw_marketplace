using Microsoft.EntityFrameworkCore;
using mutraw_marketplace_api.Data;
using mutraw_marketplace_api.Models;

namespace mutraw_marketplace_api.Repositories;

public class ProductRepo(AppDbContext appDbContext) : IRepository<Product>
{
    public async Task<Product?> GetIdAsync(long id)
    {
        return await appDbContext.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await appDbContext.Products.ToListAsync();
    }

    public async Task CreateAsync(Product entity)
    {
        appDbContext.Products.Add(entity);
        await appDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product entity)
    {
        appDbContext.Products.Update(entity);
        await appDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        appDbContext.Products.Remove(new Product { Id = id });
        await appDbContext.SaveChangesAsync();
    }

    public async Task<Product?> GetOneByCustomSqlAsync(string query)
    {
        appDbContext.Products.FromSqlRaw(query);
        return await appDbContext.Products.FirstOrDefaultAsync();
    }
}