using API_First_Lesson.Models;
using API_First_Lesson.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderEventController : ControllerBase
    {
        private readonly IOrderEventService _orderEventService;

        public OrderEventController(IOrderEventService orderEventService)
        {
            _orderEventService = orderEventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderEvents(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _orderEventService.GetAllAsync(page, pageSize, cancellationToken);
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var orderEvent = await _orderEventService.GetByIdAsync(id, cancellationToken);
            if (orderEvent is null) return NotFound();
            return Ok(orderEvent);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderEventCreateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var orderEvent = await _orderEventService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = orderEvent.OrderEventId }, orderEvent);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderEventUpdateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var orderEvent = await _orderEventService.UpdateAsync(id, dto, cancellationToken);
                if (orderEvent is null) return NotFound();
                return Ok(orderEvent);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, OrderEventPatchDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var orderEvent = await _orderEventService.PatchAsync(id, dto, cancellationToken);
                if (orderEvent is null) return NotFound();
                return Ok(orderEvent);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _orderEventService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
