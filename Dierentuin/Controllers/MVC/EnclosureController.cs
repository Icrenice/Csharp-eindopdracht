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

        // GET: /Enclosure/Index
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

        // GET: /Enclosure/Details/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (enclosure == null) return NotFound();

            return View(enclosure);
        }

        // GET: /Enclosure/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Enclosure/Create
        [HttpPost]
        public async Task<IActionResult> Create(Enclosure enclosure)
        {
            if (ModelState.IsValid)
            {
                _context.Enclosures.Add(enclosure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(enclosure);
        }

        // GET: /Enclosure/Edit/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure == null) return NotFound();
            return View(enclosure);
        }

        // POST: /Enclosure/Edit/5
        [HttpPost("{id:int}")]
        public async Task<IActionResult> Edit(int id, Enclosure enclosure)
        {
            if (id != enclosure.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(enclosure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(enclosure);
        }

        // GET: /Enclosure/Delete/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure == null) return NotFound();
            return View(enclosure);
        }

        // POST: /Enclosure/DeleteConfirmed/5
        [HttpPost("{id:int}"), ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure != null)
            {
                _context.Enclosures.Remove(enclosure);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // -----------------------------
        // Hieronder je extra acties: Sunrise, Sunset, FeedingTime, CheckConstraints
        // -----------------------------

        // GET: /Enclosure/Sunrise/5
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

        // GET: /Enclosure/Sunset/5
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

        // GET: /Enclosure/FeedingTime/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> FeedingTime(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (enclosure == null) return NotFound("Enclosure not found");

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
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Enclosure/CheckConstraints/5
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
