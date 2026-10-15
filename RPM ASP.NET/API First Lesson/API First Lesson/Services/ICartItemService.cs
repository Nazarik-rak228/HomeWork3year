using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface ICartItemService
{
    Task<(List<CartItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<CartItemDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<CartItemDTO> CreateAsync(CartItemCreateDTO dto, CancellationToken ct);

    Task<CartItemDTO?> UpdateAsync(int id, CartItemUpdateDTO dto, CancellationToken ct);

    Task<CartItemDTO?> PatchAsync(int id, CartItemPatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
