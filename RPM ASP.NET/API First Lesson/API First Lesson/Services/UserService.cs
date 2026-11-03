using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    private readonly WebAppContext _context;

    private readonly PasswordHasher<User> _hasher = new();

    public UserService(WebAppContext context,IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
    }
    public async Task<User> RegisterAsync(string username, string password) 
    {
        var all = await _context.Users.AnyAsync(u => u.Username == username);
        if (all) 
        {
            throw new InvalidOperationException($"Login {username} уже используется ");
        }

        var user = new User
        {
            Username = username,
            RoleId = 2,
            IsActive = true,
        };

        user.PasswordHash = _hasher.HashPassword(user, password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _context.Entry(user).Reference(u => u.Role).LoadAsync();
        return user;
    }

    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null) 
        {
            return null;
        }
        
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result == PasswordVerificationResult.Failed ? null: user;
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
