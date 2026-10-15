using API_First_Lesson.Models;
using API_First_Lesson.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _productService.GetAllAsync(page, pageSize, cancellationToken);
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product is null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.UpdateAsync(id, dto, cancellationToken);
                if (product is null) return NotFound();
                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, ProductPatchDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.PatchAsync(id, dto, cancellationToken);
                if (product is null) return NotFound();
                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _productService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> Reserve(int id, ReserveProductDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.ReserveAsync(id, dto, cancellationToken);
                if (product is null) return NotFound();
                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/cancel-reserve")]
        public async Task<IActionResult> CancelReserve(int id, CancelReserveDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.CancelReserveAsync(id, dto, cancellationToken);
                if (product is null) return NotFound(new { error = "Активный резерв по товару не найден." });
                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
