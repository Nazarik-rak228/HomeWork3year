using API_First_Lesson.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly WebAppContext _context;

        public CartItemController(WebAppContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetCartItems(int page = 1, int pageSize = 10)
        {
            var cartItems = _context.CartItems
                .OrderBy(ci => ci.CartItemId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(cartItems.Select(ToDto).ToList());
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartItemId == id);
            if (cartItem is null) return NotFound();

            return Ok(ToDto(cartItem));
        }

        [HttpPost]
        public IActionResult Create(CartItemCreateDTO dto)
        {
            var cartItem = new CartItem
            {
                CartId = dto.CartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            _context.CartItems.Add(cartItem);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = cartItem.CartItemId }, ToDto(cartItem));
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CartItemUpdateDTO dto)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartItemId == id);
            if (cartItem is null) return NotFound();

            cartItem.CartId = dto.CartId;
            cartItem.ProductId = dto.ProductId;
            cartItem.Quantity = dto.Quantity;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, CartItemPatchDTO dto)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartItemId == id);
            if (cartItem is null) return NotFound();

            if (dto.CartId is not null) cartItem.CartId = dto.CartId.Value;
            if (dto.ProductId is not null) cartItem.ProductId = dto.ProductId.Value;
            if (dto.Quantity is not null) cartItem.Quantity = dto.Quantity.Value;

            _context.SaveChanges();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartItemId == id);
            if (cartItem is null) return NotFound();

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return NoContent();
        }

        private CartItemDTO ToDto(CartItem cartItem)
        {
            return new CartItemDTO
            {
                CartItemId = cartItem.CartItemId,
                CartId = cartItem.CartId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity
            };
        }
    }
}