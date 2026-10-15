using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class CartItemRepository : ICartItemRepository
{
    private readonly WebAppContext _context;

    public CartItemRepository(WebAppContext context)
    {
        _context = context;
    }

    private IQueryable<CartItem> Query() => _context.CartItems
        .Include(i => i.Cart)
        .Include(i => i.Product);

    public async Task<List<CartItem>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await Query()
            .OrderBy(i => i.CartItemId)
            .Skip(Math.Max(page - 1, 0) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await Query().CountAsync(ct);
    }

    public async Task<CartItem?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await Query().FirstOrDefaultAsync(i => i.CartItemId == id, ct);
    }

    public async Task<Cart?> FindCartAsync(int cartId, CancellationToken ct)
    {
        return await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartId, ct);
    }

    public async Task<Product?> FindProductAsync(int productId, CancellationToken ct)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
    }

    public async Task AddAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
    }

    public void Update(CartItem cartItem) => _context.CartItems.Update(cartItem);

    public void Remove(CartItem cartItem) => _context.CartItems.Remove(cartItem);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
