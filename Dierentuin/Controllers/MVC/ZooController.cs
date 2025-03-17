using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.Controllers.MVC
{
    [Route("[controller]/[action]")]
    public class ZooController : Controller
    {
        private readonly AppDbContext _context;

        public ZooController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Laad de "eerste" Zoo
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .ThenInclude(a => a.Category)
                .FirstOrDefaultAsync();

            if (zoo == null) return NotFound("Geen dierentuin in DB gevonden.");

            return View(zoo);
        }

        // Sunrise
        [HttpGet]
        public async Task<IActionResult> Sunrise()
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync();
            if (zoo == null) return NotFound();

            // Diurnal: wakker, Nocturnal: slapen, Cathemeral: vrij
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
                            // willekeurig: hier kiezen we gewoon wakker
                            animal.IsAwake = true;
                            break;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Sunset
        [HttpGet]
        public async Task<IActionResult> Sunset()
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync();
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
                            // willekeurig: hier kiezen we slapen
                            animal.IsAwake = false;
                            break;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // FeedingTime
        [HttpGet]
        public async Task<IActionResult> FeedingTime()
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync();
            if (zoo == null) return NotFound();

            // Dieren voeden: "eten van andere dieren gaat altijd boven het gegeven eten"
            // => Carnivores eten eerst kleinere dieren in hetzelfde verblijf
            foreach (var enclosure in zoo.Enclosures)
            {
                // Sorteren op size (klein->groot)
                var animalsOrdered = enclosure.Animals.OrderBy(a => a.Size).ToList();
                for (int i = 0; i < animalsOrdered.Count; i++)
                {
                    var predator = animalsOrdered[i];
                    if (predator.DietaryClass == DietaryClass.Carnivore)
                    {
                        // Eet alle dieren die kleiner zijn
                        var preyList = animalsOrdered
                            .Where(a => a.Size < predator.Size)
                            .ToList();

                        foreach (var p in preyList)
                        {
                            enclosure.Animals.Remove(p);
                            _context.Animals.Remove(p);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // CheckConstraints
        [HttpGet]
        public async Task<IActionResult> CheckConstraints()
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync();
            if (zoo == null) return NotFound();

            var messages = new List<string>();

            // check: genoeg ruimte, securitylevel hoog genoeg
            foreach (var enclosure in zoo.Enclosures)
            {
                double totalNeeded = enclosure.Animals.Sum(a => a.SpaceRequirement);
                if (totalNeeded > enclosure.Size)
                {
                    messages.Add($"Enclosure '{enclosure.Name}' heeft te weinig ruimte!");
                }

                foreach (var animal in enclosure.Animals)
                {
                    if (enclosure.SecurityLevel < animal.SecurityRequirement)
                    {
                        messages.Add($"Beveiliging te laag voor {animal.Name} (in {enclosure.Name}).");
                    }
                }
            }

            TempData["ConstraintsResult"] = messages.Count > 0
                ? string.Join("<br/>", messages)
                : "Alle constraints in orde!";

            return RedirectToAction(nameof(Index));
        }

        // AutoAssign
        [HttpGet]
        public async Task<IActionResult> AutoAssign(bool removeExisting = false)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Enclosures)
                .ThenInclude(e => e.Animals)
                .FirstOrDefaultAsync();
            if (zoo == null) return NotFound();

            // 1) removeExisting => verwijder alle verblijven + indeling
            if (removeExisting)
            {
                // Let op: extreme vorm
                _context.Enclosures.RemoveRange(zoo.Enclosures);
                zoo.Enclosures.Clear();
                await _context.SaveChangesAsync();
            }

            // 2) Maak 'n nieuw verblijf:
            var newEnc = new Enclosure
            {
                Name = "Nieuw verblijf",
                Climate = Climate.Temperate,
                HabitatType = HabitatType.Grassland,
                SecurityLevel = SecurityLevel.Medium,
                Size = 200
            };
            zoo.Enclosures.Add(newEnc);

            // 3) Pak alle dieren zonder Enclosure (EnclosureId == null)
            var unassignedAnimals = await _context.Animals
                .Where(a => a.EnclosureId == null)
                .ToListAsync();

            foreach (var a in unassignedAnimals)
            {
                newEnc.Animals.Add(a);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 