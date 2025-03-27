using Dierentuin.Controllers.MVC;
using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Models.Enums;  // Als je enums gebruikt in je Animal-model
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dierentuin.Tests
{
    public class AnimalControllerTests
    {
        // Hulpfunctie: maakt voor elke test een nieuwe in-memory DbContext
        private AppDbContext GetInMemoryDbContext()
        {
            var opties = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(opties);
        }

        [Fact]
        public async Task Index_ZonderZoekterm_GeeftAlleDierenTerug()
        {
            // Arrange: Voeg twee dieren toe aan de database
            var context = GetInMemoryDbContext();
            context.Animals.Add(new Animal { Id = 1, Name = "Leeuw" });
            context.Animals.Add(new Animal { Id = 2, Name = "Olifant" });
            await context.SaveChangesAsync();

            var controller = new AnimalController(context);

            // Act: Roep de Index-methode aan zonder zoekterm
            var resultaat = await controller.Index(search: null) as ViewResult;

            // Assert: Controleer of de view teruggegeven wordt met twee dieren
            Assert.NotNull(resultaat);
            Assert.IsType<ViewResult>(resultaat);
            // Er wordt geen expliciete viewnaam meegegeven; dus conventioneel is ViewName null
            Assert.Null(resultaat.ViewName);

            var model = Assert.IsAssignableFrom<List<Animal>>(resultaat.Model);
            Assert.Equal(2, model.Count);
            Assert.Contains(model, a => a.Name == "Leeuw");
            Assert.Contains(model, a => a.Name == "Olifant");
        }

        [Fact]
        public async Task Index_MetZoekterm_FiltertDieren()
        {
            // Arrange: Voeg drie dieren toe
            var context = GetInMemoryDbContext();
            context.Animals.Add(new Animal { Id = 1, Name = "Leeuw" });
            context.Animals.Add(new Animal { Id = 2, Name = "Olifant" });
            context.Animals.Add(new Animal { Id = 3, Name = "Lemur" });
            await context.SaveChangesAsync();

            var controller = new AnimalController(context);

            // Act: Zoek op "L" (zodat zowel "Leeuw" als "Lemur" worden gevonden)
            var resultaat = await controller.Index("L") as ViewResult;

            // Assert: Controleer dat het model precies twee dieren bevat
            Assert.NotNull(resultaat);
            var model = Assert.IsType<List<Animal>>(resultaat.Model);
            Assert.Equal(2, model.Count); // Verwacht "Leeuw" en "Lemur"
            Assert.Contains(model, a => a.Name == "Leeuw");
            Assert.Contains(model, a => a.Name == "Lemur");
            Assert.DoesNotContain(model, a => a.Name == "Olifant");
        }

        [Fact]
        public async Task Details_BestaandeId_GeeftCorrectDierTerug()
        {
            // Arrange: Voeg een dier toe met Id 10
            var context = GetInMemoryDbContext();
            context.Animals.Add(new Animal { Id = 10, Name = "Zebra" });
            await context.SaveChangesAsync();

            var controller = new AnimalController(context);

            // Act: Roep de Details-methode aan met Id 10
            var resultaat = await controller.Details(10) as ViewResult;

            // Assert: Controleer of het dier correct wordt weergegeven
            Assert.NotNull(resultaat);
            var model = Assert.IsType<Animal>(resultaat.Model);
            Assert.Equal(10, model.Id);
            Assert.Equal("Zebra", model.Name);
        }

        [Fact]
        public async Task Details_NietBestaandeId_GeeftNotFoundTerug()
        {
            // Arrange: Er bestaat geen dier met Id 999
            var context = GetInMemoryDbContext();
            var controller = new AnimalController(context);

            // Act
            var resultaat = await controller.Details(999);

            // Assert: Verwacht een NotFoundResult
            Assert.IsType<NotFoundResult>(resultaat);
        }

        [Fact]
        public async Task Create_ValidModel_StuurtDoorNaarIndex()
        {
            // Arrange: Maak een nieuw dier aan
            var context = GetInMemoryDbContext();
            var controller = new AnimalController(context);
            var nieuwDier = new Animal { Name = "Giraf", Species = "Giraffa" };

            // Act: Roep de Create-methode aan
            var resultaat = await controller.Create(nieuwDier);

            // Assert: Controleer dat er naar Index wordt doorgestuurd en dat het dier is opgeslagen in de DB
            var redirect = Assert.IsType<RedirectToActionResult>(resultaat);
            Assert.Equal("Index", redirect.ActionName);

            var opgeslagenDier = context.Animals.SingleOrDefault(a => a.Name == "Giraf");
            Assert.NotNull(opgeslagenDier);
            Assert.Equal("Giraffa", opgeslagenDier.Species);
        }

        [Fact]
        public async Task Create_InvalidModel_GeeftViewTerug()
        {
            // Arrange: Maak een ongeldig dier aan (naam leeg)
            var context = GetInMemoryDbContext();
            var controller = new AnimalController(context);
            var ongeldigDier = new Animal { Name = "", Species = "Onbekend" };

            // Simuleer een fout in het model (naam is verplicht)
            controller.ModelState.AddModelError("Name", "Verplicht");

            // Act: Roep Create aan met het ongeldige dier
            var resultaat = await controller.Create(ongeldigDier);

            // Assert: Verwacht dat dezelfde view wordt weergegeven met het ongeldige model
            var viewResult = Assert.IsType<ViewResult>(resultaat);
            var teruggegevenModel = Assert.IsType<Animal>(viewResult.Model);
            Assert.Equal("", teruggegevenModel.Name);
        }

        [Fact]
        public async Task Edit_Get_BestaandeId_GeeftViewTerug()
        {
            // Arrange: Voeg een dier toe met Id 50
            var context = GetInMemoryDbContext();
            context.Animals.Add(new Animal { Id = 50, Name = "Neushoorn" });
            await context.SaveChangesAsync();

            var controller = new AnimalController(context);

            // Act: Roep Edit aan voor Id 50
            var resultaat = await controller.Edit(50) as ViewResult;

            // Assert: Controleer dat de view het juiste model bevat
            Assert.NotNull(resultaat);
            var model = Assert.IsType<Animal>(resultaat.Model);
            Assert.Equal(50, model.Id);
            Assert.Equal("Neushoorn", model.Name);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_StuurtDoorEnSlaatOp()
        {
            // Arrange: Voeg een dier toe met Id 99
            var context = GetInMemoryDbContext();
            context.Animals.Add(new Animal { Id = 99, Name = "Beer" });
            await context.SaveChangesAsync();

            // Simuleer dat het dier losgekoppeld is (zoals bij modelbinding)
            var bestaandDier = context.Animals.First(a => a.Id == 99);
            context.Entry(bestaandDier).State = EntityState.Detached;

            var controller = new AnimalController(context);
            var bijgewerktDier = new Animal { Id = 99, Name = "Poolbeer" };

            // Act: Roep de Edit-methode aan
            var resultaat = await controller.Edit(99, bijgewerktDier);

            // Assert: Controleer dat er naar Index wordt doorgestuurd en dat de naam is aangepast
            var redirect = Assert.IsType<RedirectToActionResult>(resultaat);
            Assert.Equal("Index", redirect.ActionName);
            var opgeslagenDier = context.Animals.Single(a => a.Id == 99);
            Assert.Equal("Poolbeer", opgeslagenDier.Name);
        }

        [Fact]
        public async Task Edit_Post_IdMismatch_GeeftBadRequestTerug()
        {
            // Arrange: Voeg een dier toe met Id 5
            var context = GetInMemoryDbContext();
            context.Animals.Add(new Animal { Id = 5, Name = "Koala" });
            await context.SaveChangesAsync();

            var controller = new AnimalController(context);

            // Act: Probeer te editen met een mismatch tussen route-ID en model-ID
            var resultaat = await controller.Edit(999, new Animal { Id = 5, Name = "Koala" });

            // Assert: Verwacht een BadRequestResult
            Assert.IsType<BadRequestResult>(resultaat);
        }

        [Fact]
        public async Task DeleteConfirmed_BestaandeId_VerwijdertDierEnStuurtDoor()
        {
            // Arrange: Voeg een dier toe met Id 1
            var context = GetInMemoryDbContext();
            context.Animals.Add(new Animal { Id = 1, Name = "Leeuw" });
            await context.SaveChangesAsync();

            var controller = new AnimalController(context);

            // Act: Roep DeleteConfirmed aan voor Id 1
            var resultaat = await controller.DeleteConfirmed(1);

            // Assert: Controleer dat er naar Index wordt doorgestuurd en dat het dier is verwijderd
            var redirect = Assert.IsType<RedirectToActionResult>(resultaat);
            Assert.Equal("Index", redirect.ActionName);
            Assert.False(context.Animals.Any(a => a.Id == 1));
        }
    }
}
