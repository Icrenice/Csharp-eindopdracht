using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZooApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ZooApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ZooApi (stel dat je meerdere Zoo's zou hebben?)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Zoo>>> GetAllZoos()
        {
            return await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .ToListAsync();
        }

        // GET: api/ZooApi/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Zoo>> GetZoo(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null) return NotFound();
            return zoo;
        }

        // POST: api/ZooApi
        [HttpPost]
        public async Task<ActionResult<Zoo>> CreateZoo(Zoo zoo)
        {
            _context.Zoos.Add(zoo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetZoo), new { id = zoo.Id }, zoo);
        }

        // PUT: api/ZooApi/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateZoo(int id, Zoo updated)
        {
            if (id != updated.Id) return BadRequest("ID mismatch.");

            _context.Entry(updated).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Zoos.Any(z => z.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/ZooApi/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteZoo(int id)
        {
            var zoo = await _context.Zoos.FindAsync(id);
            if (zoo == null) return NotFound();

            _context.Zoos.Remove(zoo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------- Acties ----------

        // GET: api/ZooApi/5/sunrise
        [HttpGet("{id:int}/sunrise")]
        public async Task<IActionResult> Sunrise(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null) return NotFound();

            foreach (var enclosure in zoo.Enclosures)
            {
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
            }

            await _context.SaveChangesAsync();
            return Ok($"Sunrise executed for Zoo {zoo.Name}.");
        }

        // GET: api/ZooApi/5/sunset
        [HttpGet("{id:int}/sunset")]
        public async Task<IActionResult> Sunset(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null) return NotFound();

            foreach (var enclosure in zoo.Enclosures)
            {
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
            }

            await _context.SaveChangesAsync();
            return Ok($"Sunset executed for Zoo {zoo.Name}.");
        }

        // GET: api/ZooApi/5/feedingtime
        [HttpGet("{id:int}/feedingtime")]
        public async Task<IActionResult> FeedingTime(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null) return NotFound();

            foreach (var enclosure in zoo.Enclosures)
            {
                var animalsOrdered = enclosure.Animals.OrderBy(a => a.Size).ToList();

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
            }

            await _context.SaveChangesAsync();
            return Ok($"FeedingTime executed for Zoo {zoo.Name}.");
        }

        // GET: api/ZooApi/5/checkconstraints
        [HttpGet("{id:int}/checkconstraints")]
        public async Task<IActionResult> CheckConstraints(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null) return NotFound();

            var messages = new List<string>();

            foreach (var enclosure in zoo.Enclosures)
            {
                double totalNeeded = enclosure.Animals.Sum(a => a.SpaceRequirement);
                if (totalNeeded > enclosure.Size)
                {
                    messages.Add($"Enclosure '{enclosure.Name}' has not enough space!");
                }

                foreach (var animal in enclosure.Animals)
                {
                    if (enclosure.SecurityLevel < animal.SecurityRequirement)
                    {
                        messages.Add($"Security too low for {animal.Name} in {enclosure.Name}.");
                    }
                }
            }

            if (messages.Count == 0)
            {
                messages.Add("All constraints OK!");
            }

            return Ok(messages);
        }

        // GET: api/ZooApi/5/autoassign?removeExisting=false
        [HttpGet("{id:int}/autoassign")]
        public async Task<IActionResult> AutoAssign(int id, bool removeExisting = false)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null) return NotFound();

            if (removeExisting)
            {
                _context.Enclosures.RemoveRange(zoo.Enclosures);
                zoo.Enclosures.Clear();
                await _context.SaveChangesAsync();
            }

            var newEnc = new Enclosure
            {
                Name = "New Enclosure",
                Climate = Climate.Temperate,
                HabitatType = HabitatType.Grassland,
                SecurityLevel = SecurityLevel.Medium,
                Size = 200
            };
            zoo.Enclosures.Add(newEnc);

            var unassignedAnimals = await _context.Animals
                .Where(a => a.EnclosureId == null)
                .ToListAsync();

            foreach (var a in unassignedAnimals)
            {
                newEnc.Animals.Add(a);
            }

            await _context.SaveChangesAsync();
            return Ok("AutoAssign complete.");
        }
    }
}
