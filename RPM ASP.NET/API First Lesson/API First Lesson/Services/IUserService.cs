using API_First_Lesson.Models;

namespace API_First_Lesson.Services;

public interface IUserService
{
    Task<(List<UserDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);

    Task<UserDTO?> GetByIdAsync(int id, CancellationToken ct);

    Task<UserDTO> CreateAsync(UserCreateDTO dto, CancellationToken ct);

    Task<UserDTO?> UpdateAsync(int id, UserUpdateDTO dto, CancellationToken ct);

    Task<UserDTO?> PatchAsync(int id, UserPatchDTO dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
