using API_First_Lesson.Models;

namespace API_First_Lesson.Repositories;

public interface IOrderEventRepository
{
    Task<List<OrderEvent>> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task<OrderEvent?> GetByIdAsync(int id, CancellationToken ct);

    Task<Order?> FindOrderAsync(int orderId, CancellationToken ct);

    Task<User?> FindUserAsync(int userId, CancellationToken ct);

    Task<Product?> FindProductAsync(int productId, CancellationToken ct);

    Task<OrderEvent?> FindActiveReserveAsync(int productId, CancellationToken ct);

    Task AddAsync(OrderEvent orderEvent);

    void Update(OrderEvent orderEvent);

    void Remove(OrderEvent orderEvent);

    Task SaveChangesAsync();
}
