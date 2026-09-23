using API_First_Lesson.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly WebAppContext _context;

        public RoleController(WebAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetRoles(int page = 1, int pageSize = 10)
        {
            var roles = _context.Roles
                .OrderBy(r => r.RoleId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(roles.Select(ToDto).ToList());
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleId == id);
            if (role is null) return NotFound();

            return Ok(ToDto(role));
        }

        [HttpPost]
        public IActionResult Create(RoleCreateDTO dto)
        {
            var role = new Role
            {
                RoleName = dto.RoleName
            };

            _context.Roles.Add(role);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, ToDto(role));
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, RoleUpdateDTO dto)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleId == id);
            if (role is null) return NotFound();

            role.RoleName = dto.RoleName;

            _context.SaveChanges();

            return NoContent();
        }


        [HttpPatch("{id}")]
        public IActionResult Patch(int id, RolePatchDTO dto)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleId == id);
            if (role is null) return NotFound();

            if (dto.RoleName is not null) role.RoleName = dto.RoleName;

            _context.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleId == id);
            if (role is null) return NotFound();

            _context.Roles.Remove(role);
            _context.SaveChanges();

            return NoContent();
        }

        private RoleDTO ToDto(Role role)
        {
            return new RoleDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }
    }
}