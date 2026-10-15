using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class OrderEventRepository : IOrderEventRepository
{
    private readonly WebAppContext _context;

    public OrderEventRepository(WebAppContext context)
    {
        _context = context;
    }

    private IQueryable<OrderEvent> Query() => _context.OrderEvents
        .Include(e => e.Order)
        .Include(e => e.User)
        .Include(e => e.Product);

    public async Task<List<OrderEvent>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await Query()
            .OrderBy(e => e.OrderEventId)
            .Skip(Math.Max(page - 1, 0) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await Query().CountAsync(ct);
    }

    public async Task<OrderEvent?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await Query().FirstOrDefaultAsync(e => e.OrderEventId == id, ct);
    }

    public async Task<Order?> FindOrderAsync(int orderId, CancellationToken ct)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId, ct);
    }

    public async Task<User?> FindUserAsync(int userId, CancellationToken ct)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, ct);
    }

    public async Task<Product?> FindProductAsync(int productId, CancellationToken ct)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
    }

    public async Task<OrderEvent?> FindActiveReserveAsync(int productId, CancellationToken ct)
    {
        return await Query()
            .Where(e => e.ProductId == productId && e.EventType == OrderEvent.TypeReserve)
            .OrderByDescending(e => e.OrderEventId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddAsync(OrderEvent orderEvent)
    {
        await _context.OrderEvents.AddAsync(orderEvent);
    }

    public void Update(OrderEvent orderEvent) => _context.OrderEvents.Update(orderEvent);

    public void Remove(OrderEvent orderEvent) => _context.OrderEvents.Remove(orderEvent);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
