using API_First_Lesson.Models;
using API_First_Lesson.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _orderService.GetAllAsync(page, pageSize, cancellationToken);
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetByIdAsync(id, cancellationToken);
            if (order is null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderUpdateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.UpdateAsync(id, dto, cancellationToken);
                if (order is null) return NotFound();
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, OrderPatchDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.PatchAsync(id, dto, cancellationToken);
                if (order is null) return NotFound();
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _orderService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("place")]
        public async Task<IActionResult> Place(PlaceOrderDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.PlaceAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
