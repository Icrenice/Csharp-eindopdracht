using Dierentuin.Data;
using Dierentuin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CategoryApi?search=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories([FromQuery] string? search)
        {
            var query = _context.Categories
                .Include(c => c.Animals)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search));
            }

            return await query.ToListAsync();
        }

        // GET: api/CategoryApi/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Animals)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();
            return category;
        }

        // POST: api/CategoryApi
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // PUT: api/CategoryApi/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, Category updated)
        {
            if (id != updated.Id) return BadRequest("ID mismatch.");

            _context.Entry(updated).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(c => c.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/CategoryApi/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Optioneel: extra endpoint om een dier aan categorie toe te wijzen
        // [HttpPost("{categoryId:int}/assign/{animalId:int}")]
        // public async Task<IActionResult> AssignAnimal(int categoryId, int animalId) { ... }
    }
}
