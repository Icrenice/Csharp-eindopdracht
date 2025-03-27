using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.Controllers.MVC
{
    [Route("[controller]/[action]")]
    public class AnimalController : Controller
    {
        private readonly AppDbContext _context;

        public AnimalController(AppDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // CRUD (Index, Details, Create, Edit, Delete)
        // -----------------------------

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
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

            var animals = await query.ToListAsync();
            return View(animals);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();
            return View(animal);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Enclosures = _context.Enclosures.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Animal animal)
        {
            if (ModelState.IsValid)
            {
                _context.Animals.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();
            return View(animal);
        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> Edit(int id, Animal animal)
        {
            if (id != animal.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();
            return View(animal);
        }

        [HttpPost("{id:int}"), ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // -----------------------------
        // Acties: Sunrise, Sunset, FeedingTime, CheckConstraints
        // -----------------------------

        /// <summary>
        /// Sunrise: diurnal -> awake, nocturnal -> sleep, cathemeral -> choose
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Sunrise(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure) // als je bijv. enclosure checkt
                .FirstOrDefaultAsync(a => a.Id == id);

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
                    // random of logica naar wens
                    animal.IsAwake = true;
                    break;
            }

            await _context.SaveChangesAsync();
            // evt. feedback
            TempData["AnimalActionResult"] = $"{animal.Name} is now {(animal.IsAwake ? "AWAKE" : "ASLEEP")} at sunrise.";
            return RedirectToAction(nameof(Index));
        }

     
        /// Sunset: diurnal -> sleep, nocturnal -> awake, Cathemeral -> choose
    
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Sunset(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

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
                    // random of logica
                    animal.IsAwake = false;
                    break;
            }

            await _context.SaveChangesAsync();
            TempData["AnimalActionResult"] = $"{animal.Name} is now {(animal.IsAwake ? "AWAKE" : "ASLEEP")} at sunset.";
            return RedirectToAction(nameof(Index));
        }


        /// FeedingTime: "eten van andere dieren gaat altijd boven het gegeven eten"
        /// 
        /// Bij één dier is dit vrij rudimentair. In de ZooController doen we normaliter
        /// de logic per enclosure. Hier een simpele variant:
     
        [HttpGet("{id:int}")]
        public async Task<IActionResult> FeedingTime(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();

            // Als dier Carnivore is, eet het eventuele kleinere dieren in hetzelfde Enclosure
            if (animal.DietaryClass == DietaryClass.Carnivore && animal.Enclosure != null)
            {
                // Vind alle dieren in dit verblijf die kleiner zijn
                var smallerPrey = animal.Enclosure.Animals
                    .Where(a => a.Size < animal.Size && a.Id != animal.Id)
                    .ToList();

                if (smallerPrey.Any())
                {
                    // Eet ze op (verwijder)
                    foreach (var prey in smallerPrey)
                    {
                        animal.Enclosure.Animals.Remove(prey);
                        _context.Animals.Remove(prey);
                    }
                    await _context.SaveChangesAsync();

                    TempData["AnimalActionResult"] = $"{animal.Name} ate {smallerPrey.Count} smaller animals from enclosure '{animal.Enclosure.Name}'!";
                }
                else
                {
                    TempData["AnimalActionResult"] = $"{animal.Name} (Carnivore) found no smaller prey to eat.";
                }
            }
            else
            {
                TempData["AnimalActionResult"] = $"{animal.Name} was fed normally (not carnivore or not in an enclosure).";
            }

            return RedirectToAction(nameof(Index));
        }

  
        /// CheckConstraints: bv. of 't dier genoeg ruimte heeft en of SecurityLevel
        /// in z'n Enclosure hoog genoeg is

        [HttpGet("{id:int}")]
        public async Task<IActionResult> CheckConstraints(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();

            var messages = new List<string>();

            // Check ruimte
            if (animal.Enclosure != null)
            {
                // Vergelijk de total space requirement in that enclosure
                double totalNeeded = animal.Enclosure.Animals.Sum(a => a.SpaceRequirement);
                if (totalNeeded > animal.Enclosure.Size)
                {
                    messages.Add($"Not enough space in '{animal.Enclosure.Name}'! (Needed={totalNeeded}, Available={animal.Enclosure.Size}).");
                }

                // Check security
                if (animal.SecurityRequirement > animal.Enclosure.SecurityLevel)
                {
                    messages.Add($"Security is too low for {animal.Name} (Required={animal.SecurityRequirement}, Enclosure={animal.Enclosure.SecurityLevel}).");
                }
            }
            else
            {
                messages.Add($"{animal.Name} has no enclosure assigned. Can't fully check constraints.");
            }

            if (messages.Count == 0)
            {
                messages.Add($"All constraints OK for {animal.Name}.");
            }

            TempData["AnimalConstraints"] = string.Join("<br/>", messages);
            return RedirectToAction(nameof(Index));
        }
    }
}
