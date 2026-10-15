using API_First_Lesson.Models;

namespace API_First_Lesson.Repositories;

public interface IOrderItemRepository
{
    Task<List<OrderItem>> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task<OrderItem?> GetByIdAsync(int id, CancellationToken ct);

    Task<Order?> FindOrderAsync(int orderId, CancellationToken ct);

    Task<Product?> FindProductAsync(int productId, CancellationToken ct);

    Task AddAsync(OrderItem orderItem);

    void Update(OrderItem orderItem);

    void Remove(OrderItem orderItem);

    Task SaveChangesAsync();
}
