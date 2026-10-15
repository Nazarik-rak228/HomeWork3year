using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly WebAppContext _context;

    public OrderItemRepository(WebAppContext context)
    {
        _context = context;
    }

    private IQueryable<OrderItem> Query() => _context.OrderItems
        .Include(i => i.Order)
        .Include(i => i.Product);

    public async Task<List<OrderItem>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await Query()
            .OrderBy(i => i.OrderItemId)
            .Skip(Math.Max(page - 1, 0) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await Query().CountAsync(ct);
    }

    public async Task<OrderItem?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await Query().FirstOrDefaultAsync(i => i.OrderItemId == id, ct);
    }

    public async Task<Order?> FindOrderAsync(int orderId, CancellationToken ct)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId, ct);
    }

    public async Task<Product?> FindProductAsync(int productId, CancellationToken ct)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
    }

    public async Task AddAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
    }

    public void Update(OrderItem orderItem) => _context.OrderItems.Update(orderItem);

    public void Remove(OrderItem orderItem) => _context.OrderItems.Remove(orderItem);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
