using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly WebAppContext _context;

    public CategoryRepository(WebAppContext context)
    {
        _context = context;
    }

    private IQueryable<Category> Query() => _context.Categories;

    public async Task<List<Category>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await Query()
            .OrderBy(c => c.CategoryId)
            .Skip(Math.Max(page - 1, 0) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await Query().CountAsync(ct);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await Query().FirstOrDefaultAsync(c => c.CategoryId == id, ct);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public void Update(Category category) => _context.Categories.Update(category);

    public void Remove(Category category) => _context.Categories.Remove(category);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
