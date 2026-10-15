using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface IProductService
{
    Task<(List<ProductDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<ProductDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<ProductDTO> CreateAsync(ProductCreateDTO dto, CancellationToken ct);

    Task<ProductDTO?> UpdateAsync(int id, ProductUpdateDTO dto, CancellationToken ct);

    Task<ProductDTO?> PatchAsync(int id, ProductPatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);

    Task<ProductDTO?> ReserveAsync(int id, ReserveProductDTO dto, CancellationToken ct);

    Task<ProductDTO?> CancelReserveAsync(int id, CancelReserveDTO dto, CancellationToken ct);
}
