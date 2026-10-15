using API_First_Lesson.Models;

namespace API_First_Lesson.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task<Category?> GetByIdAsync(int id, CancellationToken ct);

    Task AddAsync(Category category);

    void Update(Category category);

    void Remove(Category category);

    Task SaveChangesAsync();
}
