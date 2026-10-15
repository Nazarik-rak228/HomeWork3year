using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface ICategoryService
{
    Task<(List<CategoryDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<CategoryDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<CategoryDTO> CreateAsync(CategoryCreateDTO dto, CancellationToken ct);

    Task<CategoryDTO?> UpdateAsync(int id, CategoryUpdateDTO dto, CancellationToken ct);

    Task<CategoryDTO?> PatchAsync(int id, CategoryPatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
