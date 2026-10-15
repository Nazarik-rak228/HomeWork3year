using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface IOrderEventService
{
    Task<(List<OrderEventDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<OrderEventDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<OrderEventDTO> CreateAsync(OrderEventCreateDTO dto, CancellationToken ct);

    Task<OrderEventDTO?> UpdateAsync(int id, OrderEventUpdateDTO dto, CancellationToken ct);

    Task<OrderEventDTO?> PatchAsync(int id, OrderEventPatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
