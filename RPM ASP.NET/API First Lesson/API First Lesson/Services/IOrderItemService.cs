using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface IOrderItemService
{
    Task<(List<OrderItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<OrderItemDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<OrderItemDTO> CreateAsync(OrderItemCreateDTO dto, CancellationToken ct);

    Task<OrderItemDTO?> UpdateAsync(int id, OrderItemUpdateDTO dto, CancellationToken ct);

    Task<OrderItemDTO?> PatchAsync(int id, OrderItemPatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
