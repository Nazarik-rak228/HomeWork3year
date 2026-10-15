using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface IRoleService
{
    Task<(List<RoleDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<RoleDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<RoleDTO> CreateAsync(RoleCreateDTO dto, CancellationToken ct);

    Task<RoleDTO?> UpdateAsync(int id, RoleUpdateDTO dto, CancellationToken ct);

    Task<RoleDTO?> PatchAsync(int id, RolePatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
