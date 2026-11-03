using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly WebAppContext _context;

    public OrderRepository(WebAppContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .OrderBy(o => o.OrderId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await _context.Orders.CountAsync(ct);
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id, ct);
    }

    public async Task<User?> FindUserAsync(int userId, CancellationToken ct)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, ct);
    }

    public async Task<List<Product>> FindProductsAsync(IEnumerable<int> productIds, CancellationToken ct)
    {
        var ids = productIds.Distinct().ToList();
        return await _context.Products
            .Where(p => ids.Contains(p.ProductId))
            .ToListAsync(ct);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public void Update(Order order) => _context.Orders.Update(order);

    public void Remove(Order order) => _context.Orders.Remove(order);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
