using API_First_Lesson.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly WebAppContext _context;

        public OrderController(WebAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetOrders(int page = 1, int pageSize = 10)
        {
            var orders = _context.Orders
                .OrderBy(o => o.OrderId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(orders.Select(ToDto).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order is null) return NotFound();

            return Ok(ToDto(order));
        }
        [HttpPost]
        public IActionResult Create(OrderCreateDTO dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                TotalAmount = dto.TotalAmount,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, ToDto(order));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, OrderUpdateDTO dto)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order is null) return NotFound();

            order.UserId = dto.UserId;
            order.TotalAmount = dto.TotalAmount;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, OrderPatchDTO dto)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order is null) return NotFound();

            if (dto.UserId is not null) order.UserId = dto.UserId.Value;
            if (dto.TotalAmount is not null) order.TotalAmount = dto.TotalAmount.Value;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order is null) return NotFound();

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }

        private OrderDTO ToDto(Order order)
        {
            return new OrderDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            };
        }
    }
}