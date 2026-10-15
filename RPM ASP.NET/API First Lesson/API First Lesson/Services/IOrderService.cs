using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface IOrderService
{
    Task<(List<OrderDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<OrderDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<OrderDTO> CreateAsync(OrderCreateDTO dto, CancellationToken ct);

    Task<OrderDTO?> UpdateAsync(int id, OrderUpdateDTO dto, CancellationToken ct);

    Task<OrderDTO?> PatchAsync(int id, OrderPatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);

    Task<OrderDTO> PlaceAsync(PlaceOrderDTO dto, CancellationToken ct);
}
