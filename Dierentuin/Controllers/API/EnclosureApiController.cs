using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnclosureApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnclosureApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EnclosureApi?search=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enclosure>>> GetEnclosures([FromQuery] string? search)
        {
            var query = _context.Enclosures
                .Include(e => e.Animals)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search));
            }

            return await query.ToListAsync();
        }

        // GET: api/EnclosureApi/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Enclosure>> GetEnclosure(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null) return NotFound();
            return enclosure;
        }

        // POST: api/EnclosureApi
        [HttpPost]
        public async Task<ActionResult<Enclosure>> CreateEnclosure(Enclosure enclosure)
        {
            _context.Enclosures.Add(enclosure);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEnclosure), new { id = enclosure.Id }, enclosure);
        }

        // PUT: api/EnclosureApi/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEnclosure(int id, Enclosure updated)
        {
            if (id != updated.Id) return BadRequest("ID mismatch.");

            _context.Entry(updated).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Enclosures.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/EnclosureApi/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEnclosure(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure == null) return NotFound();

            _context.Enclosures.Remove(enclosure);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------- Acties ----------

        // GET: api/EnclosureApi/5/sunrise
        [HttpGet("{id:int}/sunrise")]
        public async Task<IActionResult> Sunrise(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (enclosure == null) return NotFound();

            foreach (var animal in enclosure.Animals)
            {
                switch (animal.ActivityPattern)
                {
                    case ActivityPattern.Diurnal:
                        animal.IsAwake = true;
                        break;
                    case ActivityPattern.Nocturnal:
                        animal.IsAwake = false;
                        break;
                    case ActivityPattern.Cathemeral:
                        animal.IsAwake = true;
                        break;
                }
            }

            await _context.SaveChangesAsync();
            return Ok($"Sunrise executed for Enclosure {enclosure.Name}.");
        }

        // GET: api/EnclosureApi/5/sunset
        [HttpGet("{id:int}/sunset")]
        public async Task<IActionResult> Sunset(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (enclosure == null) return NotFound();

            foreach (var animal in enclosure.Animals)
            {
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
            }

            await _context.SaveChangesAsync();
            return Ok($"Sunset executed for Enclosure {enclosure.Name}.");
        }

        // GET: api/EnclosureApi/5/feedingtime
        [HttpGet("{id:int}/feedingtime")]
        public async Task<IActionResult> FeedingTime(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (enclosure == null) return NotFound();

            var animalsOrdered = enclosure.Animals
                .OrderBy(a => a.Size)
                .ToList();

            for (int i = 0; i < animalsOrdered.Count; i++)
            {
                var predator = animalsOrdered[i];
                if (predator.DietaryClass == DietaryClass.Carnivore)
                {
                    var preyList = animalsOrdered
                        .Where(a => a.Size < predator.Size)
                        .ToList();

                    foreach (var prey in preyList)
                    {
                        enclosure.Animals.Remove(prey);
                        _context.Animals.Remove(prey);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok($"FeedingTime executed for Enclosure {enclosure.Name}.");
        }

        // GET: api/EnclosureApi/5/checkconstraints
        [HttpGet("{id:int}/checkconstraints")]
        public async Task<IActionResult> CheckConstraints(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (enclosure == null) return NotFound();

            var messages = new List<string>();

            double totalNeeded = enclosure.Animals.Sum(a => a.SpaceRequirement);
            if (totalNeeded > enclosure.Size)
            {
                messages.Add($"Not enough space in enclosure '{enclosure.Name}'!");
            }

            foreach (var animal in enclosure.Animals)
            {
                if (enclosure.SecurityLevel < animal.SecurityRequirement)
                {
                    messages.Add($"Security level too low for {animal.Name}.");
                }
            }

            if (messages.Count == 0)
            {
                messages.Add("All constraints OK!");
            }

            return Ok(messages);
        }
    }
}
