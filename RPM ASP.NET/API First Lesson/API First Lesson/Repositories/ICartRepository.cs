using API_First_Lesson.Models;

namespace API_First_Lesson.Repositories;

public interface ICartRepository
{
    Task<List<Cart>> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task<Cart?> GetByIdAsync(int id, CancellationToken ct);

    Task<User?> FindUserAsync(int userId, CancellationToken ct);

    Task AddAsync(Cart cart);

    void Update(Cart cart);

    void Remove(Cart cart);

    Task SaveChangesAsync();
}
