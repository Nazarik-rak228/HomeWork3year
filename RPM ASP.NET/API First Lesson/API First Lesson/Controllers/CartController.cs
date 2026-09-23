using API_First_Lesson.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly WebAppContext _context;

        public CartController(WebAppContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetCarts(int page = 1, int pageSize = 10)
        {
            var carts = _context.Carts
                .OrderBy(c => c.CartId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(carts);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == id);
            if (cart is null) return NotFound();

            return Ok(ToDto(cart));
        }

        [HttpPost]
        public IActionResult Create(CartCreateDTO dto)
        {
            var cart = new Cart
            {
                UserId = dto.UserId,
                CreatedDate = DateTime.UtcNow
            };

            _context.Carts.Add(cart);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = cart.CartId }, ToDto(cart));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CartUpdateDTO dto)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == id);
            if (cart is null) return NotFound();

            cart.UserId = dto.UserId;

            _context.SaveChanges();
            return NoContent();
        }


        [HttpPatch("{id}")]
        public IActionResult Patch(int id, CartPatchDTO dto)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == id);
            if (cart is null) return NotFound();

            if (dto.UserId is not null)
            {
                cart.UserId = dto.UserId.Value;
            }

            _context.SaveChanges();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == id);
            if (cart is null) return NotFound();

            _context.Carts.Remove(cart);
            _context.SaveChanges();

            return NoContent();
        }

        private CartDTO ToDto(Cart cart)
        {
            return new CartDTO
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CreatedDate = cart.CreatedDate
            };
        }
    }
}