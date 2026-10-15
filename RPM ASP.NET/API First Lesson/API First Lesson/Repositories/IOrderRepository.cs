using API_First_Lesson.Models;

namespace API_First_Lesson.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task<Order?> GetByIdAsync(int id, CancellationToken ct);

    Task<User?> FindUserAsync(int userId, CancellationToken ct);

    Task<List<Product>> FindProductsAsync(IEnumerable<int> productIds, CancellationToken ct);

    Task AddAsync(Order order);

    void Update(Order order);

    void Remove(Order order);

    Task SaveChangesAsync();
}
