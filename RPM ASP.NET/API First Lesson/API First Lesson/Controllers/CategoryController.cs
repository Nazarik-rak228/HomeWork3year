using API_First_Lesson.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly WebAppContext _context;

        public CategoryController(WebAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCategories(int page = 1, int pageSize = 10)
        {
            var categories = _context.Categories
                .OrderBy(c => c.CategoryId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(categories.Select(ToDto).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category is null) return NotFound();

            return Ok(ToDto(category));
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateDTO dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, ToDto(category));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CategoryUpdateDTO dto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category is null) return NotFound();

            category.CategoryName = dto.CategoryName;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, CategoryPatchDTO dto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category is null) return NotFound();

            if (dto.CategoryName is not null) category.CategoryName = dto.CategoryName;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category is null) return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }

        private CategoryDTO ToDto(Category category)
        {
            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }
    }
}