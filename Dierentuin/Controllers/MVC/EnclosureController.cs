using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.Controllers.MVC
{
    [Route("[controller]/[action]")]
    public class EnclosureController : Controller
    {
        private readonly AppDbContext _context;

        public EnclosureController(AppDbContext context)
        {
            _context = context;
        }

        // Voorbeeld: GET /Enclosure/Index + filter
        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Enclosures
                .Include(e => e.Animals)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search));
            }

            var enclosures = await query.ToListAsync();
            return View(enclosures);
        }

        // ... (de CRUD-acties zoals Edit, Details, etc.)

        // Sunrise op verblijf-niveau
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Sunrise(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null) return NotFound("Enclosure not found");

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
            return RedirectToAction(nameof(Index));
        }

        // Sunset op verblijf-niveau
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Sunset(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null) return NotFound("Enclosure not found");

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
            return RedirectToAction(nameof(Index));
        }

        // FeedingTime op verblijf-niveau
        [HttpGet("{id:int}")]
        public async Task<IActionResult> FeedingTime(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null) return NotFound("Enclosure not found");

            var animalsOrdered = enclosure.Animals
                .OrderBy(a => a.Size)
                .ToList();

            // Carnivores eten eerst kleinere dieren
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
            return RedirectToAction(nameof(Index));
        }

        // CheckConstraints op verblijf-niveau
        [HttpGet("{id:int}")]
        public async Task<IActionResult> CheckConstraints(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null) return NotFound("Enclosure not found");

            var messages = new List<string>();

            double totalNeeded = enclosure.Animals.Sum(a => a.SpaceRequirement);
            if (totalNeeded > enclosure.Size)
            {
                messages.Add($"Not enough space in '{enclosure.Name}'. Needed: {totalNeeded}");
            }

            foreach (var animal in enclosure.Animals)
            {
                if (enclosure.SecurityLevel < animal.SecurityRequirement)
                {
                    messages.Add($"Security too low for {animal.Name} in {enclosure.Name}.");
                }
            }

            TempData["EnclosureCheck"] = messages.Count > 0
                ? string.Join("<br/>", messages)
                : "All constraints OK!";

            return RedirectToAction(nameof(Index));
        }
    }
}
