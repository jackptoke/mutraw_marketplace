using Microsoft.EntityFrameworkCore;
using mutraw_marketplace_api.Data;
using mutraw_marketplace_api.Models;

namespace mutraw_marketplace_api.Repositories;

public class ProductCategoryRepo(AppDbContext appDbContext) : IRepository<ProductCategory>
{
    public async Task<ProductCategory?> GetIdAsync(long id)
    {
        return await appDbContext.ProductCategories.FindAsync(id);
    }

    public async Task<IEnumerable<ProductCategory>> GetAllAsync()
    {
        return await appDbContext.ProductCategories.ToListAsync();
    }

    public async Task CreateAsync(ProductCategory entity)
    {
        appDbContext.ProductCategories.Add(entity);
        await appDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductCategory entity)
    {
        appDbContext.ProductCategories.Update(entity);
        await appDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        appDbContext.ProductCategories.Remove(new ProductCategory { Id = id });
        await appDbContext.SaveChangesAsync();
    }

    public async Task<ProductCategory?> GetOneByCustomSqlAsync(string query)
    {
        return await appDbContext.ProductCategories.FromSqlRaw(query).FirstOrDefaultAsync();
    }
}