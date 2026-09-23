using API_First_Lesson.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly WebAppContext _context;

        public ProductController(WebAppContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetProducts(int page = 1, int pageSize = 10)
        {
            var products = _context.Products
                .OrderBy(p => p.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(products.Select(ToDto).ToList());
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product is null) return NotFound();

            return Ok(ToDto(product));
        }


        [HttpPost]
        public IActionResult Create(ProductCreateDTO dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                Discription = dto.Discription,
                ImageData = dto.ImageData,
                ImageMimeType = dto.ImageMimeType
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, ToDto(product));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductUpdateDTO dto)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product is null) return NotFound();

            product.ProductName = dto.ProductName;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.Discription = dto.Discription;
            product.ImageData = dto.ImageData;
            product.ImageMimeType = dto.ImageMimeType;

            _context.SaveChanges();

            return NoContent();
        }

      
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, ProductPatchDTO dto)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product is null) return NotFound();

            if (dto.ProductName is not null) product.ProductName = dto.ProductName;
            if (dto.Price is not null) product.Price = dto.Price.Value;
            if (dto.CategoryId is not null) product.CategoryId = dto.CategoryId.Value;
            if (dto.Discription is not null) product.Discription = dto.Discription;
            if (dto.ImageData is not null) product.ImageData = dto.ImageData;
            if (dto.ImageMimeType is not null) product.ImageMimeType = dto.ImageMimeType;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product is null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }

        private ProductDTO ToDto(Product product)
        {
            return new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Discription = product.Discription,
                ImageData = product.ImageData,
                ImageMimeType = product.ImageMimeType
            };
        }
    }
}