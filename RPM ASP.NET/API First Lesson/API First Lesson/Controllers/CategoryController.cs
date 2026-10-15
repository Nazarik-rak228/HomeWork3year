using API_First_Lesson.Models;
using API_First_Lesson.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_First_Lesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _categoryService.GetAllAsync(page, pageSize, cancellationToken);
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            if (category is null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryService.UpdateAsync(id, dto, cancellationToken);
                if (category is null) return NotFound();
                return Ok(category);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, CategoryPatchDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryService.PatchAsync(id, dto, cancellationToken);
                if (category is null) return NotFound();
                return Ok(category);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _categoryService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
