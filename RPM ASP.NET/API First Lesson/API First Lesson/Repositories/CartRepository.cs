using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class CartRepository : ICartRepository
{
    private readonly WebAppContext _context;

    public CartRepository(WebAppContext context)
    {
        _context = context;
    }

    private IQueryable<Cart> Query() => _context.Carts
        .Include(c => c.User)
        .Include(c => c.CartItems)
            .ThenInclude(i => i.Product);

    public async Task<List<Cart>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await Query()
            .OrderBy(c => c.CartId)
            .Skip(Math.Max(page - 1, 0) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await Query().CountAsync(ct);
    }

    public async Task<Cart?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await Query().FirstOrDefaultAsync(c => c.CartId == id, ct);
    }

    public async Task<User?> FindUserAsync(int userId, CancellationToken ct)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, ct);
    }

    public async Task AddAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    public void Update(Cart cart) => _context.Carts.Update(cart);

    public void Remove(Cart cart) => _context.Carts.Remove(cart);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
