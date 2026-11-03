using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly WebAppContext _context;

    public ProductRepository(WebAppContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.ProductId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await _context.Products.CountAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id, ct);
    }

    public async Task<Category?> FindCategoryAsync(int categoryId, CancellationToken ct)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId, ct);
    }

    public async Task<List<Product>> FindManyAsync(IEnumerable<int> productIds, CancellationToken ct)
    {
        var ids = productIds.Distinct().ToList();
        return await _context.Products
            .Where(p => ids.Contains(p.ProductId))
            .ToListAsync(ct);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Update(Product product) => _context.Products.Update(product);

    public void Remove(Product product) => _context.Products.Remove(product);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
