using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface ICartService
{
    Task<(List<CartDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<CartDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<CartDTO> CreateAsync(CartCreateDTO dto, CancellationToken ct);

    Task<CartDTO?> UpdateAsync(int id, CartUpdateDTO dto, CancellationToken ct);

    Task<CartDTO?> PatchAsync(int id, CartPatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
