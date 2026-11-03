using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class UserRepository : IUserRepository
{
    private readonly WebAppContext _context;

    public UserRepository(WebAppContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Cart)
            .OrderBy(u => u.UserId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await _context.Users.CountAsync(ct);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Cart)
            .FirstOrDefaultAsync(u => u.UserId == id, ct);
    }

    public async Task<Role?> FindRoleAsync(int roleId, CancellationToken ct)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId, ct);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public void Update(User user) => _context.Users.Update(user);

    public void Remove(User user) => _context.Users.Remove(user);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
