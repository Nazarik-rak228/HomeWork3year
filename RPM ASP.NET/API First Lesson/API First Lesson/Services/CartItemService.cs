using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using AutoMapper;

namespace API_First_Lesson.Services;

public class CartItemService : ICartItemService
{
    private readonly ICartItemRepository _repository;
    private readonly IMapper _mapper;

    public CartItemService(ICartItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<CartItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var cartItems = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);

        return (_mapper.Map<List<CartItemDTO>>(cartItems), total);
    }

    public async Task<CartItemDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var cartItem = await _repository.GetByIdAsync(id, ct);
        return cartItem is null ? null : _mapper.Map<CartItemDTO>(cartItem);
    }

    public async Task<CartItemDTO> CreateAsync(CartItemCreateDTO dto, CancellationToken ct)
    {
        _ = await _repository.FindCartAsync(dto.CartId, ct)
            ?? throw new InvalidOperationException($"Корзина с id {dto.CartId} не найдена.");

        _ = await _repository.FindProductAsync(dto.ProductId, ct)
            ?? throw new InvalidOperationException($"Товар с id {dto.ProductId} не найден.");

        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Количество должно быть больше нуля.");

        var cartItem = _mapper.Map<CartItem>(dto);

        await _repository.AddAsync(cartItem);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(cartItem.CartItemId, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить только что созданную позицию корзины.");

        return _mapper.Map<CartItemDTO>(saved);
    }

    public async Task<CartItemDTO?> UpdateAsync(int id, CartItemUpdateDTO dto, CancellationToken ct)
    {
        var cartItem = await _repository.GetByIdAsync(id, ct);
        if (cartItem is null) return null;

        _ = await _repository.FindCartAsync(dto.CartId, ct)
            ?? throw new InvalidOperationException($"Корзина с id {dto.CartId} не найдена.");

        _ = await _repository.FindProductAsync(dto.ProductId, ct)
            ?? throw new InvalidOperationException($"Товар с id {dto.ProductId} не найден.");

        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Количество должно быть больше нуля.");

        _mapper.Map(dto, cartItem);
        _repository.Update(cartItem);
        await _repository.SaveChangesAsync();

        return _mapper.Map<CartItemDTO>(cartItem);
    }

    public async Task<CartItemDTO?> PatchAsync(int id, CartItemPatchDTO dto, CancellationToken ct)
    {
        var cartItem = await _repository.GetByIdAsync(id, ct);
        if (cartItem is null) return null;

        if (dto.CartId is not null)
        {
            _ = await _repository.FindCartAsync(dto.CartId.Value, ct)
                ?? throw new InvalidOperationException($"Корзина с id {dto.CartId} не найдена.");
            cartItem.CartId = dto.CartId.Value;
        }
        if (dto.ProductId is not null)
        {
            _ = await _repository.FindProductAsync(dto.ProductId.Value, ct)
                ?? throw new InvalidOperationException($"Товар с id {dto.ProductId} не найден.");
            cartItem.ProductId = dto.ProductId.Value;
        }
        if (dto.Quantity is not null)
        {
            if (dto.Quantity <= 0) throw new InvalidOperationException("Количество должно быть больше нуля.");
            cartItem.Quantity = dto.Quantity.Value;
        }

        _repository.Update(cartItem);
        await _repository.SaveChangesAsync();

        return _mapper.Map<CartItemDTO>(cartItem);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var cartItem = await _repository.GetByIdAsync(id, ct);
        if (cartItem is null) return false;

        _repository.Remove(cartItem);
        await _repository.SaveChangesAsync();

        return true;
    }
}
