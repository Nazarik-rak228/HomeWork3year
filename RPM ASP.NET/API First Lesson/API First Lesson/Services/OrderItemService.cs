using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using AutoMapper;

namespace API_First_Lesson.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IOrderItemRepository _repository;
    private readonly IMapper _mapper;

    public OrderItemService(IOrderItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<OrderItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var orderItems = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);

        return (_mapper.Map<List<OrderItemDTO>>(orderItems), total);
    }

    public async Task<OrderItemDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var orderItem = await _repository.GetByIdAsync(id, ct);
        return orderItem is null ? null : _mapper.Map<OrderItemDTO>(orderItem);
    }

    public async Task<OrderItemDTO> CreateAsync(OrderItemCreateDTO dto, CancellationToken ct)
    {
        _ = await _repository.FindOrderAsync(dto.OrderId, ct)
            ?? throw new InvalidOperationException($"Заказ с id {dto.OrderId} не найден.");

        _ = await _repository.FindProductAsync(dto.ProductId, ct)
            ?? throw new InvalidOperationException($"Товар с id {dto.ProductId} не найден.");

        ValidateQuantityAndPrice(dto.Quantity, dto.PriceAtPurchase);

        var orderItem = _mapper.Map<OrderItem>(dto);

        await _repository.AddAsync(orderItem);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(orderItem.OrderItemId, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить только что созданную позицию заказа.");

        return _mapper.Map<OrderItemDTO>(saved);
    }

    public async Task<OrderItemDTO?> UpdateAsync(int id, OrderItemUpdateDTO dto, CancellationToken ct)
    {
        var orderItem = await _repository.GetByIdAsync(id, ct);
        if (orderItem is null) return null;

        _ = await _repository.FindOrderAsync(dto.OrderId, ct)
            ?? throw new InvalidOperationException($"Заказ с id {dto.OrderId} не найден.");

        _ = await _repository.FindProductAsync(dto.ProductId, ct)
            ?? throw new InvalidOperationException($"Товар с id {dto.ProductId} не найден.");

        ValidateQuantityAndPrice(dto.Quantity, dto.PriceAtPurchase);

        _mapper.Map(dto, orderItem);
        _repository.Update(orderItem);
        await _repository.SaveChangesAsync();

        return _mapper.Map<OrderItemDTO>(orderItem);
    }

    public async Task<OrderItemDTO?> PatchAsync(int id, OrderItemPatchDTO dto, CancellationToken ct)
    {
        var orderItem = await _repository.GetByIdAsync(id, ct);
        if (orderItem is null) return null;

        if (dto.OrderId is not null)
        {
            _ = await _repository.FindOrderAsync(dto.OrderId.Value, ct)
                ?? throw new InvalidOperationException($"Заказ с id {dto.OrderId} не найден.");
            orderItem.OrderId = dto.OrderId.Value;
        }
        if (dto.ProductId is not null)
        {
            _ = await _repository.FindProductAsync(dto.ProductId.Value, ct)
                ?? throw new InvalidOperationException($"Товар с id {dto.ProductId} не найден.");
            orderItem.ProductId = dto.ProductId.Value;
        }
        if (dto.Quantity is not null)
        {
            if (dto.Quantity <= 0) throw new InvalidOperationException("Количество должно быть больше нуля.");
            orderItem.Quantity = dto.Quantity.Value;
        }
        if (dto.PriceAtPurchase is not null)
        {
            if (dto.PriceAtPurchase < 0) throw new InvalidOperationException("Цена не может быть отрицательной.");
            orderItem.PriceAtPurchase = dto.PriceAtPurchase.Value;
        }

        _repository.Update(orderItem);
        await _repository.SaveChangesAsync();

        return _mapper.Map<OrderItemDTO>(orderItem);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var orderItem = await _repository.GetByIdAsync(id, ct);
        if (orderItem is null) return false;

        _repository.Remove(orderItem);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static void ValidateQuantityAndPrice(int quantity, decimal priceAtPurchase)
    {
        if (quantity <= 0) throw new InvalidOperationException("Количество должно быть больше нуля.");
        if (priceAtPurchase < 0) throw new InvalidOperationException("Цена не может быть отрицательной.");
    }
}
