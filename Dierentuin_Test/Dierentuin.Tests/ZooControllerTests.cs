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
    public class ZooControllerTests
    {
        // Helper: Create a new in-memory AppDbContext.
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // INDEX Tests

        [Fact]
        public async Task Index_NoZooFound_ReturnsNotFound()
        {
            // Arrange: No Zoo in DB.
            var context = GetInMemoryDbContext();
            var controller = new ZooController(context);

            // Act.
            var result = await controller.Index();

            // Assert: Expect NotFound with message.
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Geen dierentuin in DB gevonden.", notFound.Value);
        }

        [Fact]
        public async Task Index_ZooFound_ReturnsViewWithZoo()
        {
            // Arrange: Create a Zoo with one Enclosure and one Animal.
            var context = GetInMemoryDbContext();
            var zoo = new Zoo
            {
                Id = 1,
                Name = "Test Zoo",
                Enclosures = new List<Enclosure>
                {
                    new Enclosure
                    {
                        Id = 10,
                        Name = "Enclosure1",
                        Size = 100,
                        SecurityLevel = SecurityLevel.Medium,
                        Animals = new List<Animal>
                        {
                            new Animal
                            {
                                Id = 100,
                                Name = "Lion",
                                Species = "Panthera leo",
                                ActivityPattern = ActivityPattern.Diurnal,
                                Size = (Dierentuin.Models.Enums.Size)50,
                                DietaryClass = DietaryClass.Carnivore,
                                SpaceRequirement = 10,
                                SecurityRequirement = SecurityLevel.Medium
                            }
                        }
                    }
                }
            };
            context.Zoos.Add(zoo);
            await context.SaveChangesAsync();

            var controller = new ZooController(context);

            // Act.
            var result = await controller.Index() as ViewResult;

            // Assert.
            Assert.NotNull(result);
            var model = Assert.IsType<Zoo>(result.Model);
            Assert.Equal("Test Zoo", model.Name);
            Assert.Single(model.Enclosures);
        }

        // Sunrise Test

        [Fact]
        public async Task Sunrise_UpdatesAnimalAwakeStates_RedirectsToIndex()
        {
            // Arrange: Create a Zoo with an enclosure containing animals with different patterns.
            var context = GetInMemoryDbContext();
            var enclosure = new Enclosure
            {
                Id = 20,
                Name = "EnclosureSunrise",
                Size = 100,
                SecurityLevel = SecurityLevel.High,
                Animals = new List<Animal>
                {
                    new Animal { Id = 201, Name = "Lion", ActivityPattern = ActivityPattern.Diurnal, IsAwake = false },
                    new Animal { Id = 202, Name = "Owl", ActivityPattern = ActivityPattern.Nocturnal, IsAwake = true },
                    new Animal { Id = 203, Name = "Cat", ActivityPattern = ActivityPattern.Cathemeral, IsAwake = false }
                }
            };
            var zoo = new Zoo
            {
                Id = 2,
                Name = "ZooSunrise",
                Enclosures = new List<Enclosure> { enclosure }
            };
            context.Zoos.Add(zoo);
            await context.SaveChangesAsync();

            var controller = new ZooController(context);

            // Act.
            var result = await controller.Sunrise();

            // Assert.
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            // Verify: Diurnal -> awake; Nocturnal -> asleep; Cathemeral -> awake.
            var lion = context.Animals.First(a => a.Id == 201);
            var owl = context.Animals.First(a => a.Id == 202);
            var cat = context.Animals.First(a => a.Id == 203);
            Assert.True(lion.IsAwake);
            Assert.False(owl.IsAwake);
            Assert.True(cat.IsAwake);
        }

        // Sunset Test

        [Fact]
        public async Task Sunset_UpdatesAnimalAwakeStates_RedirectsToIndex()
        {
            // Arrange: Create a Zoo with an enclosure with animals.
            var context = GetInMemoryDbContext();
            var enclosure = new Enclosure
            {
                Id = 30,
                Name = "EnclosureSunset",
                Size = 100,
                SecurityLevel = SecurityLevel.High,
                Animals = new List<Animal>
                {
                    new Animal { Id = 301, Name = "Lion", ActivityPattern = ActivityPattern.Diurnal, IsAwake = true },
                    new Animal { Id = 302, Name = "Owl", ActivityPattern = ActivityPattern.Nocturnal, IsAwake = false },
                    new Animal { Id = 303, Name = "Cat", ActivityPattern = ActivityPattern.Cathemeral, IsAwake = true }
                }
            };
            var zoo = new Zoo
            {
                Id = 3,
                Name = "ZooSunset",
                Enclosures = new List<Enclosure> { enclosure }
            };
            context.Zoos.Add(zoo);
            await context.SaveChangesAsync();

            var controller = new ZooController(context);

            // Act.
            var result = await controller.Sunset();

            // Assert.
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            // Verify: Diurnal -> asleep; Nocturnal -> awake; Cathemeral -> asleep.
            var lion = context.Animals.First(a => a.Id == 301);
            var owl = context.Animals.First(a => a.Id == 302);
            var cat = context.Animals.First(a => a.Id == 303);
            Assert.False(lion.IsAwake);
            Assert.True(owl.IsAwake);
            Assert.False(cat.IsAwake);
        }

        // FeedingTime Test

        [Fact]
        public async Task FeedingTime_CarnivoresEatSmallerAnimals_RedirectsToIndex()
        {
            // Arrange: Create a Zoo with one enclosure with animals of various sizes/dietary classes.
            var context = GetInMemoryDbContext();
            var enclosure = new Enclosure
            {
                Id = 40,
                Name = "EnclosureFeeding",
                Size = 150,
                SecurityLevel = SecurityLevel.High,
                Animals = new List<Animal>
                {
                    new Animal { Id = 401, Name = "Tiger", DietaryClass = DietaryClass.Carnivore, Size = (Dierentuin.Models.Enums.Size)50, SpaceRequirement = 10 },
                    new Animal { Id = 402, Name = "Wolf", DietaryClass = DietaryClass.Carnivore, Size = (Dierentuin.Models.Enums.Size)30, SpaceRequirement = 8 },
                    new Animal { Id = 403, Name = "Deer", DietaryClass = DietaryClass.Herbivore, Size = (Dierentuin.Models.Enums.Size)20, SpaceRequirement = 5 },
                    new Animal { Id = 404, Name = "Buffalo", DietaryClass = DietaryClass.Herbivore, Size = (Dierentuin.Models.Enums.Size)60, SpaceRequirement = 15 }
                }
            };
            var zoo = new Zoo
            {
                Id = 4,
                Name = "ZooFeeding",
                Enclosures = new List<Enclosure> { enclosure }
            };
            context.Zoos.Add(zoo);
            await context.SaveChangesAsync();

            var controller = new ZooController(context);

            // Act.
            var result = await controller.FeedingTime();

            // Assert.
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            var remainingAnimals = context.Animals.ToList();
            // With ordering by size: Deer (20), Wolf (30), Tiger (50), Buffalo (60).
            // According to logic, Tiger (carnivore) eats all animals smaller than itself.
            Assert.Contains(remainingAnimals, a => a.Name == "Tiger");
            Assert.Contains(remainingAnimals, a => a.Name == "Buffalo");
            Assert.DoesNotContain(remainingAnimals, a => a.Name == "Wolf");
            Assert.DoesNotContain(remainingAnimals, a => a.Name == "Deer");
        }

        // CheckConstraints Test

        [Fact]
        public async Task CheckConstraints_TooSmallSpaceAndLowSecurity_SetsMessagesAndRedirects()
        {
            // Arrange: Create a Zoo with one enclosure where total required space > Size and security too low.
            var context = GetInMemoryDbContext();
            var enclosure = new Enclosure
            {
                Id = 50,
                Name = "EnclosureConstraints",
                Size = 30,  // available area as double
                SecurityLevel = SecurityLevel.Medium
            };
            enclosure.Animals.Add(new Animal
            {
                Id = 501,
                Name = "Elephant",
                SpaceRequirement = 25,
                SecurityRequirement = SecurityLevel.High,
            });
            enclosure.Animals.Add(new Animal
            {
                Id = 502,
                Name = "Giraffe",
                SpaceRequirement = 15,
                SecurityRequirement = SecurityLevel.Medium
            });
            var zoo = new Zoo
            {
                Id = 5,
                Name = "ZooConstraints",
                Enclosures = new List<Enclosure> { enclosure }
            };
            context.Zoos.Add(zoo);
            await context.SaveChangesAsync();

            var controller = new ZooController(context);
            // Initialize TempData using the FakeTempDataProvider defined in a shared location.
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider());

            // Act.
            var result = await controller.CheckConstraints();

            // Assert.
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.True(controller.TempData.ContainsKey("ConstraintsResult"));
            var messages = controller.TempData["ConstraintsResult"] as string;
            Assert.NotNull(messages);
            Assert.Contains("Enclosure 'EnclosureConstraints' heeft te weinig ruimte!", messages);
            Assert.Contains("Beveiliging te laag voor Elephant", messages);
        }

        // AutoAssign Test

        [Fact]
        public async Task AutoAssign_RemovesExistingAndAssignsUnassignedAnimals_RedirectsToIndex()
        {
            // Arrange: Create a Zoo with one existing enclosure and some unassigned animals.
            var context = GetInMemoryDbContext();
            var existingEnclosure = new Enclosure
            {
                Id = 60,
                Name = "OldEnclosure",
                Size = 100,
                SecurityLevel = SecurityLevel.Low,
                Animals = new List<Animal>
                {
                    new Animal { Id = 601, Name = "Monkey", SpaceRequirement = 5 }
                }
            };
            var zoo = new Zoo
            {
                Id = 6,
                Name = "ZooAutoAssign",
                Enclosures = new List<Enclosure> { existingEnclosure }
            };
            context.Zoos.Add(zoo);
            context.Animals.Add(new Animal { Id = 602, Name = "Parrot", SpaceRequirement = 2, EnclosureId = null });
            context.Animals.Add(new Animal { Id = 603, Name = "Elephant", SpaceRequirement = 20, EnclosureId = null });
            await context.SaveChangesAsync();

            var controller = new ZooController(context);

            // Act: Call AutoAssign with removeExisting = true.
            var result = await controller.AutoAssign(removeExisting: true);

            // Assert.
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            var updatedZoo = context.Zoos.Include(z => z.Enclosures).ThenInclude(e => e.Animals).First();
            Assert.Single(updatedZoo.Enclosures);
            var newEnclosure = updatedZoo.Enclosures.First();
            Assert.Equal("Nieuw verblijf", newEnclosure.Name);
            Assert.All(context.Animals.Where(a => a.Id == 602 || a.Id == 603), a => Assert.Equal(newEnclosure.Id, a.EnclosureId));
        }
    }
}
