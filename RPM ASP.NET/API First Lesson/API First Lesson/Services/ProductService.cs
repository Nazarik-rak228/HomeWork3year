using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using AutoMapper;

namespace API_First_Lesson.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IOrderEventRepository _orderEvents;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IOrderEventRepository orderEvents, IMapper mapper)
    {
        _repository = repository;
        _orderEvents = orderEvents;
        _mapper = mapper;
    }

    public async Task<(List<ProductDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var products = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);

        return (_mapper.Map<List<ProductDTO>>(products), total);
    }

    public async Task<ProductDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        return product is null ? null : _mapper.Map<ProductDTO>(product);
    }

    public async Task<ProductDTO> CreateAsync(ProductCreateDTO dto, CancellationToken ct)
    {
        _ = await _repository.FindCategoryAsync(dto.CategoryId, ct)
            ?? throw new InvalidOperationException($"Категория с id {dto.CategoryId} не найдена.");

        ValidatePriceAndStock(dto.Price, dto.Stock);

        var product = _mapper.Map<Product>(dto);

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(product.ProductId, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить только что созданный товар.");

        return _mapper.Map<ProductDTO>(saved);
    }

    public async Task<ProductDTO?> UpdateAsync(int id, ProductUpdateDTO dto, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return null;

        _ = await _repository.FindCategoryAsync(dto.CategoryId, ct)
            ?? throw new InvalidOperationException($"Категория с id {dto.CategoryId} не найдена.");

        ValidatePriceAndStock(dto.Price, dto.Stock);

        _mapper.Map(dto, product);
        _repository.Update(product);
        await _repository.SaveChangesAsync();

        return _mapper.Map<ProductDTO>(product);
    }

    public async Task<ProductDTO?> PatchAsync(int id, ProductPatchDTO dto, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return null;

        if (dto.ProductName is not null) product.ProductName = dto.ProductName;
        if (dto.Price is not null)
        {
            if (dto.Price < 0) throw new InvalidOperationException("Цена не может быть отрицательной.");
            product.Price = dto.Price.Value;
        }
        if (dto.Stock is not null)
        {
            if (dto.Stock < 0) throw new InvalidOperationException("Остаток на складе не может быть отрицательным.");
            product.Stock = dto.Stock.Value;
        }
        if (dto.CategoryId is not null)
        {
            _ = await _repository.FindCategoryAsync(dto.CategoryId.Value, ct)
                ?? throw new InvalidOperationException($"Категория с id {dto.CategoryId} не найдена.");
            product.CategoryId = dto.CategoryId.Value;
        }
        if (dto.Discription is not null) product.Discription = dto.Discription;
        if (dto.ImageData is not null) product.ImageData = dto.ImageData;
        if (dto.ImageMimeType is not null) product.ImageMimeType = dto.ImageMimeType;

        _repository.Update(product);
        await _repository.SaveChangesAsync();

        return _mapper.Map<ProductDTO>(product);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return false;

        _repository.Remove(product);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<ProductDTO?> ReserveAsync(int id, ReserveProductDTO dto, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return null;

        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Количество должно быть больше нуля.");

        if (product.Stock < dto.Quantity)
            throw new InvalidOperationException(
                $"Недостаточно товара \"{product.ProductName}\" на складе: остаток {product.Stock}, запрошено {dto.Quantity}.");

        _ = await _orderEvents.FindUserAsync(dto.UserId, ct)
            ?? throw new InvalidOperationException($"Пользователь с id {dto.UserId} не найден.");

        product.Stock -= dto.Quantity;

        await _orderEvents.AddAsync(new OrderEvent
        {
            OrderId = null,
            UserId = dto.UserId,
            ProductId = product.ProductId,
            Quantity = dto.Quantity,
            EventType = OrderEvent.TypeReserve,
            EventDate = DateTime.UtcNow
        });

        _repository.Update(product);

        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить товар после резервирования.");

        return _mapper.Map<ProductDTO>(saved);
    }

    public async Task<ProductDTO?> CancelReserveAsync(int id, CancelReserveDTO dto, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return null;

        var reserve = await _orderEvents.FindActiveReserveAsync(id, ct);
        if (reserve is null) return null;

        _ = await _orderEvents.FindUserAsync(dto.UserId, ct)
            ?? throw new InvalidOperationException($"Пользователь с id {dto.UserId} не найден.");

        product.Stock += reserve.Quantity;
        reserve.EventType = OrderEvent.TypeCancelled;
        reserve.EventDate = DateTime.UtcNow;

        _repository.Update(product);
        _orderEvents.Update(reserve);

        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить товар после снятия резерва.");

        return _mapper.Map<ProductDTO>(saved);
    }

    private static void ValidatePriceAndStock(decimal price, int stock)
    {
        if (price < 0) throw new InvalidOperationException("Цена не может быть отрицательной.");
        if (stock < 0) throw new InvalidOperationException("Остаток на складе не может быть отрицательным.");
    }
}
