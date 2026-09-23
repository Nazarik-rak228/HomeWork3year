using API_First_Lesson.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly WebAppContext _context;

        public OrderItemController(WebAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetOrderItems(int page = 1, int pageSize = 10)
        {
            var orderItems = _context.OrderItems
                .OrderBy(oi => oi.OrderItemId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(orderItems.Select(ToDto).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var orderItem = _context.OrderItems.FirstOrDefault(oi => oi.OrderItemId == id);
            if (orderItem is null) return NotFound();

            return Ok(ToDto(orderItem));
        }

        [HttpPost]
        public IActionResult Create(OrderItemCreateDTO dto)
        {
            var orderItem = new OrderItem
            {
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                PriceAtPurchase = dto.PriceAtPurchase
            };

            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = orderItem.OrderItemId }, ToDto(orderItem));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, OrderItemUpdateDTO dto)
        {
            var orderItem = _context.OrderItems.FirstOrDefault(oi => oi.OrderItemId == id);
            if (orderItem is null) return NotFound();

            orderItem.OrderId = dto.OrderId;
            orderItem.ProductId = dto.ProductId;
            orderItem.Quantity = dto.Quantity;
            orderItem.PriceAtPurchase = dto.PriceAtPurchase;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, OrderItemPatchDTO dto)
        {
            var orderItem = _context.OrderItems.FirstOrDefault(oi => oi.OrderItemId == id);
            if (orderItem is null) return NotFound();

            if (dto.OrderId is not null) orderItem.OrderId = dto.OrderId.Value;
            if (dto.ProductId is not null) orderItem.ProductId = dto.ProductId.Value;
            if (dto.Quantity is not null) orderItem.Quantity = dto.Quantity.Value;
            if (dto.PriceAtPurchase is not null) orderItem.PriceAtPurchase = dto.PriceAtPurchase.Value;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var orderItem = _context.OrderItems.FirstOrDefault(oi => oi.OrderItemId == id);
            if (orderItem is null) return NotFound();

            _context.OrderItems.Remove(orderItem);
            _context.SaveChanges();

            return NoContent();
        }

        private OrderItemDTO ToDto(OrderItem orderItem)
        {
            return new OrderItemDTO
            {
                OrderItemId = orderItem.OrderItemId,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                PriceAtPurchase = orderItem.PriceAtPurchase
            };
        }
    }
}