using API_First_Lesson.Models;
using API_First_Lesson.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _cartItemService.GetAllAsync(page, pageSize, cancellationToken);
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var cartItem = await _cartItemService.GetByIdAsync(id, cancellationToken);
            if (cartItem is null) return NotFound();
            return Ok(cartItem);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CartItemCreateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var cartItem = await _cartItemService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = cartItem.CartItemId }, cartItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CartItemUpdateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var cartItem = await _cartItemService.UpdateAsync(id, dto, cancellationToken);
                if (cartItem is null) return NotFound();
                return Ok(cartItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, CartItemPatchDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var cartItem = await _cartItemService.PatchAsync(id, dto, cancellationToken);
                if (cartItem is null) return NotFound();
                return Ok(cartItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _cartItemService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
