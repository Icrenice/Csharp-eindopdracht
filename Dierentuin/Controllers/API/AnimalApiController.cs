using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnimalApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AnimalApi?search=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAllAnimals([FromQuery] string? search)
        {
            var query = _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a =>
                    a.Name.Contains(search) ||
                    a.Species.Contains(search) ||
                    (a.Category != null && a.Category.Name.Contains(search)) ||
                    (a.Enclosure != null && a.Enclosure.Name.Contains(search)));
            }

            return await query.ToListAsync();
        }

        // GET: api/AnimalApi/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();

            return animal;
        }

        // POST: api/AnimalApi
        [HttpPost]
        public async Task<ActionResult<Animal>> CreateAnimal(Animal animal)
        {
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        // PUT: api/AnimalApi/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAnimal(int id, Animal updated)
        {
            if (id != updated.Id) return BadRequest("ID mismatch.");

            _context.Entry(updated).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Animals.Any(a => a.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/AnimalApi/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ---------- Acties ----------

        // GET: api/AnimalApi/5/sunrise
        [HttpGet("{id:int}/sunrise")]
        public async Task<IActionResult> Sunrise(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            switch (animal.ActivityPattern)
            {
                case ActivityPattern.Diurnal:
                    animal.IsAwake = true;
                    break;
                case ActivityPattern.Nocturnal:
                    animal.IsAwake = false;
                    break;
                case ActivityPattern.Cathemeral:
                    animal.IsAwake = true; // willekeur of random
                    break;
            }

            await _context.SaveChangesAsync();
            return Ok($"Sunrise executed for Animal {animal.Name}. IsAwake={animal.IsAwake}");
        }

        // GET: api/AnimalApi/5/sunset
        [HttpGet("{id:int}/sunset")]
        public async Task<IActionResult> Sunset(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            switch (animal.ActivityPattern)
            {
                case ActivityPattern.Diurnal:
                    animal.IsAwake = false;
                    break;
                case ActivityPattern.Nocturnal:
                    animal.IsAwake = true;
                    break;
                case ActivityPattern.Cathemeral:
                    animal.IsAwake = false;
                    break;
            }

            await _context.SaveChangesAsync();
            return Ok($"Sunset executed for Animal {animal.Name}. IsAwake={animal.IsAwake}");
        }

        // GET: api/AnimalApi/5/feedingtime
        [HttpGet("{id:int}/feedingtime")]
        public async Task<IActionResult> FeedingTime(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();

            if (animal.DietaryClass == DietaryClass.Carnivore && animal.Enclosure != null)
            {
                // Eet eerst kleinere dieren
                var smallerPrey = animal.Enclosure.Animals
                    .Where(a => a.Size < animal.Size && a.Id != animal.Id)
                    .ToList();

                foreach (var prey in smallerPrey)
                {
                    animal.Enclosure.Animals.Remove(prey);
                    _context.Animals.Remove(prey);
                }

                await _context.SaveChangesAsync();
                return Ok($"{animal.Name} ate {smallerPrey.Count} smaller animals.");
            }

            return Ok($"{animal.Name} was fed normally or has no enclosure.");
        }

        // GET: api/AnimalApi/5/checkconstraints
        [HttpGet("{id:int}/checkconstraints")]
        public async Task<IActionResult> CheckConstraints(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();

            var messages = new List<string>();

            // check ruimte & security
            if (animal.Enclosure != null)
            {
                double totalSpaceNeeded = animal.Enclosure.Animals.Sum(a => a.SpaceRequirement);
                if (totalSpaceNeeded > animal.Enclosure.Size)
                {
                    messages.Add($"Not enough space in enclosure {animal.Enclosure.Name}!");
                }

                if (animal.SecurityRequirement > animal.Enclosure.SecurityLevel)
                {
                    messages.Add($"Security level too low for {animal.Name}.");
                }
            }
            else
            {
                messages.Add($"Animal {animal.Name} is not assigned to any enclosure.");
            }

            if (messages.Count == 0)
                messages.Add($"All constraints OK for {animal.Name}.");

            return Ok(messages);
        }
    }
}
