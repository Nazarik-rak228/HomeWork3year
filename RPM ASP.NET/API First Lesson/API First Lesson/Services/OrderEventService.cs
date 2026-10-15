using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using AutoMapper;

namespace API_First_Lesson.Services;

public class OrderEventService : IOrderEventService
{
    private readonly IOrderEventRepository _repository;
    private readonly IMapper _mapper;

    public OrderEventService(IOrderEventRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<OrderEventDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var events = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);

        return (_mapper.Map<List<OrderEventDTO>>(events), total);
    }

    public async Task<OrderEventDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var orderEvent = await _repository.GetByIdAsync(id, ct);
        return orderEvent is null ? null : _mapper.Map<OrderEventDTO>(orderEvent);
    }

    public async Task<OrderEventDTO> CreateAsync(OrderEventCreateDTO dto, CancellationToken ct)
    {
        await ValidateLinksAsync(dto.OrderId, dto.UserId, dto.ProductId, dto.Quantity, dto.EventType, ct);

        var orderEvent = _mapper.Map<OrderEvent>(dto);
        orderEvent.EventDate = DateTime.UtcNow;

        await _repository.AddAsync(orderEvent);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(orderEvent.OrderEventId, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить только что созданную запись операции.");

        return _mapper.Map<OrderEventDTO>(saved);
    }

    public async Task<OrderEventDTO?> UpdateAsync(int id, OrderEventUpdateDTO dto, CancellationToken ct)
    {
        var orderEvent = await _repository.GetByIdAsync(id, ct);
        if (orderEvent is null) return null;

        await ValidateLinksAsync(dto.OrderId, dto.UserId, dto.ProductId, dto.Quantity, dto.EventType, ct);

        _mapper.Map(dto, orderEvent);
        _repository.Update(orderEvent);
        await _repository.SaveChangesAsync();

        return _mapper.Map<OrderEventDTO>(orderEvent);
    }

    public async Task<OrderEventDTO?> PatchAsync(int id, OrderEventPatchDTO dto, CancellationToken ct)
    {
        var orderEvent = await _repository.GetByIdAsync(id, ct);
        if (orderEvent is null) return null;

        if (dto.OrderId is not null)
        {
            _ = await _repository.FindOrderAsync(dto.OrderId.Value, ct)
                ?? throw new InvalidOperationException($"Заказ с id {dto.OrderId} не найден.");
            orderEvent.OrderId = dto.OrderId.Value;
        }
        if (dto.UserId is not null)
        {
            _ = await _repository.FindUserAsync(dto.UserId.Value, ct)
                ?? throw new InvalidOperationException($"Пользователь с id {dto.UserId} не найден.");
            orderEvent.UserId = dto.UserId.Value;
        }
        if (dto.ProductId is not null)
        {
            _ = await _repository.FindProductAsync(dto.ProductId.Value, ct)
                ?? throw new InvalidOperationException($"Товар с id {dto.ProductId} не найден.");
            orderEvent.ProductId = dto.ProductId.Value;
        }
        if (dto.Quantity is not null)
        {
            if (dto.Quantity <= 0) throw new InvalidOperationException("Количество должно быть больше нуля.");
            orderEvent.Quantity = dto.Quantity.Value;
        }
        if (dto.EventType is not null)
        {
            if (dto.EventType != OrderEvent.TypeReserve && dto.EventType != OrderEvent.TypeCancelled)
                throw new InvalidOperationException("Тип операции должен быть \"Reserve\" или \"Cancelled\".");
            orderEvent.EventType = dto.EventType;
        }

        _repository.Update(orderEvent);
        await _repository.SaveChangesAsync();

        return _mapper.Map<OrderEventDTO>(orderEvent);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var orderEvent = await _repository.GetByIdAsync(id, ct);
        if (orderEvent is null) return false;

        _repository.Remove(orderEvent);
        await _repository.SaveChangesAsync();

        return true;
    }

    private async Task ValidateLinksAsync(int? orderId, int? userId, int productId, int quantity, string? eventType, CancellationToken ct)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Количество должно быть больше нуля.");

        if (eventType != OrderEvent.TypeReserve && eventType != OrderEvent.TypeCancelled)
            throw new InvalidOperationException("Тип операции должен быть \"Reserve\" или \"Cancelled\".");

        _ = await _repository.FindProductAsync(productId, ct)
            ?? throw new InvalidOperationException($"Товар с id {productId} не найден.");

        if (orderId is not null)
        {
            _ = await _repository.FindOrderAsync(orderId.Value, ct)
                ?? throw new InvalidOperationException($"Заказ с id {orderId} не найден.");
        }

        if (userId is not null)
        {
            _ = await _repository.FindUserAsync(userId.Value, ct)
                ?? throw new InvalidOperationException($"Пользователь с id {userId} не найден.");
        }
    }
}
