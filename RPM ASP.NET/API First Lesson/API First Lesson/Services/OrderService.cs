using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using AutoMapper;

namespace API_First_Lesson.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IProductRepository _products;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository repository, IProductRepository products, IMapper mapper)
    {
        _repository = repository;
        _products = products;
        _mapper = mapper;
    }

    public async Task<(List<OrderDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var orders = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);

        return (_mapper.Map<List<OrderDTO>>(orders), total);
    }

    public async Task<OrderDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(id, ct);
        return order is null ? null : _mapper.Map<OrderDTO>(order);
    }

    public async Task<OrderDTO> CreateAsync(OrderCreateDTO dto, CancellationToken ct)
    {
        _ = await _repository.FindUserAsync(dto.UserId, ct)
            ?? throw new InvalidOperationException($"Пользователь с id {dto.UserId} не найден.");

        if (dto.TotalAmount < 0)
            throw new InvalidOperationException("Сумма заказа не может быть отрицательной.");

        var order = _mapper.Map<Order>(dto);
        order.OrderDate = DateTime.UtcNow;
        order.Status = OrderStatuses.Pending;

        await _repository.AddAsync(order);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(order.OrderId, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить только что созданный заказ.");

        return _mapper.Map<OrderDTO>(saved);
    }

    public async Task<OrderDTO?> UpdateAsync(int id, OrderUpdateDTO dto, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(id, ct);
        if (order is null) return null;

        _ = await _repository.FindUserAsync(dto.UserId, ct)
            ?? throw new InvalidOperationException($"Пользователь с id {dto.UserId} не найден.");

        if (dto.TotalAmount < 0)
            throw new InvalidOperationException("Сумма заказа не может быть отрицательной.");

        _mapper.Map(dto, order);
        _repository.Update(order);
        await _repository.SaveChangesAsync();

        return _mapper.Map<OrderDTO>(order);
    }

    public async Task<OrderDTO?> PatchAsync(int id, OrderPatchDTO dto, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(id, ct);
        if (order is null) return null;

        if (dto.UserId is not null)
        {
            _ = await _repository.FindUserAsync(dto.UserId.Value, ct)
                ?? throw new InvalidOperationException($"Пользователь с id {dto.UserId} не найден.");
            order.UserId = dto.UserId.Value;
        }
        if (dto.TotalAmount is not null)
        {
            if (dto.TotalAmount < 0) throw new InvalidOperationException("Сумма заказа не может быть отрицательной.");
            order.TotalAmount = dto.TotalAmount.Value;
        }

        _repository.Update(order);
        await _repository.SaveChangesAsync();

        return _mapper.Map<OrderDTO>(order);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(id, ct);
        if (order is null) return false;

        _repository.Remove(order);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<OrderDTO> PlaceAsync(PlaceOrderDTO dto, CancellationToken ct)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            throw new InvalidOperationException("Заказ должен содержать хотя бы одну позицию.");

        _ = await _repository.FindUserAsync(dto.UserId, ct)
            ?? throw new InvalidOperationException($"Пользователь с id {dto.UserId} не найден.");

        var products = await _repository.FindProductsAsync(dto.Items.Select(i => i.ProductId), ct);
        var productsById = products.ToDictionary(p => p.ProductId);

        var order = new Order
        {
            UserId = dto.UserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatuses.Pending,
            TotalAmount = 0m
        };

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
                throw new InvalidOperationException("Количество в позиции заказа должно быть больше нуля.");

            if (!productsById.TryGetValue(item.ProductId, out var product))
                throw new InvalidOperationException($"Товар с id {item.ProductId} не найден.");

            if (product.Stock < item.Quantity)
                throw new InvalidOperationException(
                    $"Недостаточно товара \"{product.ProductName}\" на складе: остаток {product.Stock}, запрошено {item.Quantity}.");

            product.Stock -= item.Quantity;
            order.TotalAmount += product.Price * item.Quantity;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                Quantity = item.Quantity,
                PriceAtPurchase = product.Price
            });

            order.OrderEvents.Add(new OrderEvent
            {
                UserId = dto.UserId,
                ProductId = product.ProductId,
                Quantity = item.Quantity,
                EventType = OrderEvent.TypeReserve,
                EventDate = DateTime.UtcNow
            });
        }

        await _repository.AddAsync(order);
        foreach (var product in productsById.Values) _products.Update(product);

        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(order.OrderId, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить только что созданный заказ.");

        return _mapper.Map<OrderDTO>(saved);
    }
}
