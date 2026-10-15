using API_First_Lesson.Models;

namespace API_First_Lesson.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task<Product?> GetByIdAsync(int id, CancellationToken ct);

    Task<Category?> FindCategoryAsync(int categoryId, CancellationToken ct);

    Task<List<Product>> FindManyAsync(IEnumerable<int> productIds, CancellationToken ct);

    Task AddAsync(Product product);

    void Update(Product product);

    void Remove(Product product);

    Task SaveChangesAsync();
}
