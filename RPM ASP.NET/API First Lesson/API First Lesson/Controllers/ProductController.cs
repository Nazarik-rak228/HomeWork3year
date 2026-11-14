using API_First_Lesson.Models;
using API_First_Lesson.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _productService.GetAllAsync(page, pageSize, cancellationToken);
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(items);
        }

        // v2: тот же сервис, но изменён контракт ответа — объект { meta, data }
        // с вложенными блоками product/category, pricing, availability и image.
        // В Swagger обе реализации представлены одним GET с выбором api-version.
        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetProductsV2(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _productService.GetAllAsync(page, pageSize, cancellationToken);
            Response.Headers["X-Total-Count"] = total.ToString();

            var response = new ProductV2ListDTO
            {
                Meta = new ProductV2MetaDTO
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
                },
                Data = items.Select(MapToV2).ToList()
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product is null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
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
        [Authorize(Roles = "Admin,Manager")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
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
        [Authorize(Roles = "Admin,Manager")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
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
        [Authorize(Roles = "Admin,Manager")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _productService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/reserve")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
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
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
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

        private static ProductV2DTO MapToV2(ProductDTO product) => new()
        {
            Product = new ProductV2InfoDTO
            {
                Id = product.ProductId,
                Name = product.ProductName,
                Description = product.Discription,
                Category = new ProductV2CategoryDTO
                {
                    Id = product.CategoryId,
                    Name = product.CategoryName ?? "Без категории"
                }
            },
            Pricing = new ProductV2PricingDTO
            {
                Value = product.Price,
                Currency = "RUB"
            },
            Availability = new ProductV2AvailabilityDTO
            {
                InStock = product.Stock > 0,
                Quantity = product.Stock
            },
            Image = new ProductV2ImageDTO
            {
                HasImage = product.ImageData is { Length: > 0 },
                MimeType = product.ImageMimeType
            }
        };
    }
}
