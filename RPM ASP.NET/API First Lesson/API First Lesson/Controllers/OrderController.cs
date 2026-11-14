using API_First_Lesson.Models;
using API_First_Lesson.Services;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    [Authorize] // все методы требуют токен
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAuthorizationService _authorizationService;

        public OrderController(IOrderService orderService, IAuthorizationService authorizationService)
        {
            _orderService = orderService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")] // только админ/менеджер видит все заказы
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

            // resource-based проверка
            var authResult = await _authorizationService.AuthorizeAsync(User, order, "OrderOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                // принудительно ставим текущего пользователя
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                dto.UserId = userId; // убедитесь, что в DTO есть UserId

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
            var existing = await _orderService.GetByIdAsync(id, cancellationToken);
            if (existing is null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, existing, "OrderOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            try
            {
                var order = await _orderService.UpdateAsync(id, dto, cancellationToken);
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
            var existing = await _orderService.GetByIdAsync(id, cancellationToken);
            if (existing is null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, existing, "OrderOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            try
            {
                var order = await _orderService.PatchAsync(id, dto, cancellationToken);
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")] // удалять может только Admin
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                dto.UserId = userId; // если есть поле

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