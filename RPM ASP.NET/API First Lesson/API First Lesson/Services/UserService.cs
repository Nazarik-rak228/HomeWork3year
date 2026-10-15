using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using AutoMapper;

namespace API_First_Lesson.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<UserDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var users = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);

        return (_mapper.Map<List<UserDTO>>(users), total);
    }

    public async Task<UserDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        return user is null ? null : _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO> CreateAsync(UserCreateDTO dto, CancellationToken ct)
    {
        _ = await _repository.FindRoleAsync(dto.RoleId, ct)
            ?? throw new InvalidOperationException($"Роль с id {dto.RoleId} не найдена.");

        var user = _mapper.Map<User>(dto);

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(user.UserId, ct)
            ?? throw new InvalidOperationException("Не удалось загрузить только что созданного пользователя.");

        return _mapper.Map<UserDTO>(saved);
    }

    public async Task<UserDTO?> UpdateAsync(int id, UserUpdateDTO dto, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return null;

        _ = await _repository.FindRoleAsync(dto.RoleId, ct)
            ?? throw new InvalidOperationException($"Роль с id {dto.RoleId} не найдена.");

        _mapper.Map(dto, user);
        _repository.Update(user);
        await _repository.SaveChangesAsync();

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO?> PatchAsync(int id, UserPatchDTO dto, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return null;

        if (dto.Username is not null) user.Username = dto.Username;
        if (dto.PasswordHash is not null) user.PasswordHash = dto.PasswordHash;
        if (dto.IsActive is not null) user.IsActive = dto.IsActive.Value;

        if (dto.RoleId is not null)
        {
            _ = await _repository.FindRoleAsync(dto.RoleId.Value, ct)
                ?? throw new InvalidOperationException($"Роль с id {dto.RoleId} не найдена.");
            user.RoleId = dto.RoleId.Value;
        }

        _repository.Update(user);
        await _repository.SaveChangesAsync();

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return false;

        _repository.Remove(user);
        await _repository.SaveChangesAsync();

        return true;
    }
}
