using API_First_Lesson.Models;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly WebAppContext _context;

    public RoleRepository(WebAppContext context)
    {
        _context = context;
    }

    private IQueryable<Role> Query() => _context.Roles;

    public async Task<List<Role>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await Query()
            .OrderBy(r => r.RoleId)
            .Skip(Math.Max(page - 1, 0) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await Query().CountAsync(ct);
    }

    public async Task<Role?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await Query().FirstOrDefaultAsync(r => r.RoleId == id, ct);
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public void Update(Role role) => _context.Roles.Update(role);

    public void Remove(Role role) => _context.Roles.Remove(role);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
