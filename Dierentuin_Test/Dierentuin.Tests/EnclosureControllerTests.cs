using Dierentuin.Controllers.MVC;
using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dierentuin.Tests
{
    // A simple fake TempData provider for testing purposes.
    public class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    public class EnclosureControllerTests
    {
        // Helper to create a fresh in-memory AppDbContext
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Index_NoSearch_ReturnsAllEnclosures()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Enclosures.Add(new Enclosure { Id = 1, Name = "Savannah" });
            context.Enclosures.Add(new Enclosure { Id = 2, Name = "Jungle" });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Index(null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Enclosure>>(result.Model);
            Assert.Equal(2, model.Count);
            Assert.Contains(model, e => e.Name == "Savannah");
            Assert.Contains(model, e => e.Name == "Jungle");
        }

        [Fact]
        public async Task Index_WithSearch_FiltersEnclosures()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Enclosures.Add(new Enclosure { Id = 1, Name = "Savannah" });
            context.Enclosures.Add(new Enclosure { Id = 2, Name = "Arctic" });
            context.Enclosures.Add(new Enclosure { Id = 3, Name = "Safari" });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);

            // Act: searching for "Sa" (capital S) should match "Savannah" and "Safari"
            var result = await controller.Index("Sa") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Enclosure>>(result.Model);
            Assert.Equal(2, model.Count);
            Assert.Contains(model, e => e.Name == "Savannah");
            Assert.Contains(model, e => e.Name == "Safari");
            Assert.DoesNotContain(model, e => e.Name == "Arctic");
        }

        [Fact]
        public async Task Details_ExistingId_ReturnsEnclosure()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Enclosures.Add(new Enclosure { Id = 10, Name = "Desert" });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Details(10) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Enclosure>(result.Model);
            Assert.Equal(10, model.Id);
            Assert.Equal("Desert", model.Name);
        }

        [Fact]
        public async Task Details_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidEnclosure_RedirectsToIndex()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EnclosureController(context);
            var newEnclosure = new Enclosure { Name = "Ocean" };

            // Act
            var result = await controller.Create(newEnclosure);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(context.Enclosures.SingleOrDefault(e => e.Name == "Ocean"));
        }

        [Fact]
        public async Task Create_InvalidEnclosure_ReturnsView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EnclosureController(context);
            controller.ModelState.AddModelError("Name", "Required");
            var invalidEnclosure = new Enclosure { Name = "" };

            // Act
            var result = await controller.Create(invalidEnclosure);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Enclosure>(viewResult.Model);
            Assert.Equal("", model.Name);
        }

        [Fact]
        public async Task Edit_Get_ExistingId_ReturnsViewWithModel()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Enclosures.Add(new Enclosure { Id = 20, Name = "Mountains" });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Edit(20) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Enclosure>(result.Model);
            Assert.Equal("Mountains", model.Name);
            Assert.Equal(20, model.Id);
        }

        [Fact]
        public async Task Edit_Get_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_Valid_RedirectsAndSaves()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Enclosures.Add(new Enclosure { Id = 30, Name = "InitialName" });
            await context.SaveChangesAsync();
            var tracked = context.Enclosures.First(e => e.Id == 30);
            context.Entry(tracked).State = EntityState.Detached;

            var controller = new EnclosureController(context);
            var updated = new Enclosure { Id = 30, Name = "UpdatedName" };

            // Act
            var result = await controller.Edit(30, updated);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            var encl = context.Enclosures.First(e => e.Id == 30);
            Assert.Equal("UpdatedName", encl.Name);
        }

        [Fact]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Enclosures.Add(new Enclosure { Id = 5, Name = "Mismatch" });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);
            var updated = new Enclosure { Id = 5, Name = "Mismatch2" };

            // Act
            var result = await controller.Edit(999, updated);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_Get_ExistingId_ReturnsView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Enclosures.Add(new Enclosure { Id = 40, Name = "ToDelete" });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Delete(40) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Enclosure>(result.Model);
            Assert.Equal(40, model.Id);
            Assert.Equal("ToDelete", model.Name);
        }

        [Fact]
        public async Task Delete_Get_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ExistingId_RedirectsAndRemovesEnclosure()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Enclosures.Add(new Enclosure { Id = 50, Name = "DeleteMe" });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);

            // Act
            var result = await controller.DeleteConfirmed(50);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.False(context.Enclosures.Any(e => e.Id == 50));
        }

        // -----------------------------
        // Custom Actions: Sunrise, Sunset, FeedingTime, CheckConstraints
        // -----------------------------

        [Fact]
        public async Task Sunrise_UpdatesAnimalAwakeStates_RedirectsToIndex()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var enclosure = new Enclosure { Id = 1, Name = "DawnTest" };
            context.Enclosures.Add(enclosure);
            context.Animals.Add(new Animal
            {
                Id = 101,
                Name = "Lion",
                ActivityPattern = ActivityPattern.Diurnal,
                Enclosure = enclosure
            });
            context.Animals.Add(new Animal
            {
                Id = 102,
                Name = "Owl",
                ActivityPattern = ActivityPattern.Nocturnal,
                Enclosure = enclosure
            });
            context.Animals.Add(new Animal
            {
                Id = 103,
                Name = "Cat",
                ActivityPattern = ActivityPattern.Cathemeral,
                Enclosure = enclosure
            });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Sunrise(1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            var lion = context.Animals.First(a => a.Id == 101);
            var owl = context.Animals.First(a => a.Id == 102);
            var cat = context.Animals.First(a => a.Id == 103);
            Assert.True(lion.IsAwake);
            Assert.False(owl.IsAwake);
            Assert.True(cat.IsAwake);
        }

        [Fact]
        public async Task Sunset_UpdatesAnimalAwakeStates_RedirectsToIndex()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var enclosure = new Enclosure { Id = 2, Name = "SunsetTest" };
            context.Enclosures.Add(enclosure);
            context.Animals.Add(new Animal
            {
                Id = 201,
                Name = "Lion",
                ActivityPattern = ActivityPattern.Diurnal,
                Enclosure = enclosure
            });
            context.Animals.Add(new Animal
            {
                Id = 202,
                Name = "Owl",
                ActivityPattern = ActivityPattern.Nocturnal,
                Enclosure = enclosure
            });
            context.Animals.Add(new Animal
            {
                Id = 203,
                Name = "Cat",
                ActivityPattern = ActivityPattern.Cathemeral,
                Enclosure = enclosure
            });
            await context.SaveChangesAsync();

            var controller = new EnclosureController(context);

            // Act
            var result = await controller.Sunset(2);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            var lion = context.Animals.First(a => a.Id == 201);
            var owl = context.Animals.First(a => a.Id == 202);
            var cat = context.Animals.First(a => a.Id == 203);
            Assert.False(lion.IsAwake);
            Assert.True(owl.IsAwake);
            Assert.False(cat.IsAwake);
        }

        [Fact]
        public async Task FeedingTime_CarnivoresEatSmallerAnimals_RedirectsToIndex()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var enclosure = new Enclosure { Id = 3, Name = "FeedingTest" };
            context.Enclosures.Add(enclosure);
            // Add animals with different sizes and dietary classes.
            context.Animals.Add(new Animal
            {
                Id = 301,
                Name = "Tiger",
                DietaryClass = DietaryClass.Carnivore,
                Size = (Size)50,
                Enclosure = enclosure
            });
            context.Animals.Add(new Animal
            {
                Id = 302,
                Name = "Wolf",
                DietaryClass = DietaryClass.Carnivore,
                Size = (Size)30,
                Enclosure = enclosure
            });
            context.Animals.Add(new Animal
            {
                Id = 303,
                Name = "Deer",
                DietaryClass = DietaryClass.Herbivore,
                Size = (Size)20,
                Enclosure = enclosure
            });
            context.Animals.Add(new Animal
            {
                Id = 304,
                Name = "Buffalo",
                DietaryClass = DietaryClass.Herbivore,
                Size = (Size)60,
                Enclosure = enclosure
            });
            await context.SaveChangesAsync();
            var controller = new EnclosureController(context);

            // Act
            var result = await controller.FeedingTime(3);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            var animalsLeft = context.Animals.ToList();
            // Based on the logic, ordering by Size (Deer:20, Wolf:30, Tiger:50, Buffalo:60)
            // Tiger (carnivore) eats all animals smaller than itself.
            Assert.Contains(animalsLeft, a => a.Name == "Tiger");
            Assert.Contains(animalsLeft, a => a.Name == "Buffalo");
            Assert.DoesNotContain(animalsLeft, a => a.Name == "Wolf");
            Assert.DoesNotContain(animalsLeft, a => a.Name == "Deer");
        }

        [Fact]
        public async Task CheckConstraints_TooSmallSpaceAndLowSecurity_SetsMessagesAndRedirects()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var enclosure = new Enclosure
            {
                Id = 4,
                Name = "ConstraintTest",
                Size = 30,  // assign as double
                SecurityLevel = (SecurityLevel)2
            };
            context.Enclosures.Add(enclosure);
            context.Animals.Add(new Animal
            {
                Id = 401,
                Name = "Elephant",
                SpaceRequirement = 25,
                SecurityRequirement = (SecurityLevel)3,
                Enclosure = enclosure
            });
            context.Animals.Add(new Animal
            {
                Id = 402,
                Name = "Giraffe",
                SpaceRequirement = 15,
                SecurityRequirement = (SecurityLevel)2,
                Enclosure = enclosure
            });
            await context.SaveChangesAsync();
            var controller = new EnclosureController(context);
            // Initialize TempData to avoid null-reference issues.
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider());

            // Act
            var result = await controller.CheckConstraints(4);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.True(controller.TempData.ContainsKey("EnclosureCheck"));
            var messages = controller.TempData["EnclosureCheck"] as string;
            Assert.NotNull(messages);
            Assert.Contains("Not enough space", messages);
            // Update assertion to check for "Security too low" (instead of "Security level too low")
            Assert.Contains("Security too low", messages);
        }
    }
}
