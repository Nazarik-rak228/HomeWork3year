using API_First_Lesson.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly WebAppContext _context;

        public UserController(WebAppContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetUsers(int page = 1, int pageSize = 10)
        {
            var users = _context.Users
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(users.Select(ToDto).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user is null) return NotFound();

            return Ok(ToDto(user));
        }


        [HttpPost]
        public IActionResult Create(UserCreateDTO dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = dto.PasswordHash,
                RoleId = dto.RoleId,
                IsActive = dto.IsActive
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, ToDto(user));
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, UserUpdateDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user is null) return NotFound();

            user.Username = dto.Username;
            user.PasswordHash = dto.PasswordHash;
            user.RoleId = dto.RoleId;
            user.IsActive = dto.IsActive;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, UserPatchDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user is null) return NotFound();

            if (dto.Username is not null) user.Username = dto.Username;
            if (dto.PasswordHash is not null) user.PasswordHash = dto.PasswordHash;
            if (dto.RoleId is not null) user.RoleId = dto.RoleId.Value;
            if (dto.IsActive is not null) user.IsActive = dto.IsActive.Value;

            _context.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user is null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }

        private UserDTO ToDto(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                RoleId = user.RoleId,
                IsActive = user.IsActive
            };
        }
    }
}