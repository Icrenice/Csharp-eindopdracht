using Dierentuin.Controllers.MVC;
using Dierentuin.Data;
using Dierentuin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dierentuin.Tests
{
    public class CategoryControllerTests
    {
        // Create a fresh InMemory DbContext for each test
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Index_NoSearch_ReturnsAllCategories()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 1, Name = "Mammals" });
            context.Categories.Add(new Category { Id = 2, Name = "Birds" });
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Act
            var result = await controller.Index(null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Category>>(result.Model);
            Assert.Equal(2, model.Count); // 2 categories
            Assert.Contains(model, c => c.Name == "Mammals");
            Assert.Contains(model, c => c.Name == "Birds");
        }

        [Fact]
        public async Task Index_WithSearch_FiltersCategories()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 1, Name = "Mammals" });
            context.Categories.Add(new Category { Id = 2, Name = "Birds" });
            context.Categories.Add(new Category { Id = 3, Name = "Reptiles" });
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Act: search for "ma" should only match "Mammals"
            var result = await controller.Index("ma") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Category>>(result.Model);
            Assert.Single(model);
            Assert.Equal("Mammals", model.First().Name);
        }

        [Fact]
        public async Task Details_ExistingId_ReturnsCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 10, Name = "Amphibians" });
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Act
            var result = await controller.Details(10) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Category>(result.Model);
            Assert.Equal(10, model.Id);
            Assert.Equal("Amphibians", model.Name);
        }

        [Fact]
        public async Task Details_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new CategoryController(context);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new CategoryController(context);

            var newCategory = new Category { Name = "Fish" };

            // Act
            var result = await controller.Create(newCategory);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            // Check if it's in the DB
            Assert.Single(context.Categories);
            Assert.Equal("Fish", context.Categories.First().Name);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new CategoryController(context);
            var invalidCategory = new Category { Name = "" }; // Assume Name is required

            // Force model validation error
            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await controller.Create(invalidCategory);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Category>(viewResult.Model);
            Assert.Equal("", model.Name);
        }

        [Fact]
        public async Task Edit_Get_ExistingId_ReturnsView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 20, Name = "Insects" });
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Act
            var result = await controller.Edit(20) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Category>(result.Model);
            Assert.Equal(20, model.Id);
            Assert.Equal("Insects", model.Name);
        }

        [Fact]
        public async Task Edit_Post_Valid_RedirectsToIndex()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 30, Name = "OldName" });
            await context.SaveChangesAsync();

            // Detach so EF won't complain about 2 tracked entities with the same key
            var tracked = context.Categories.First(c => c.Id == 30);
            context.Entry(tracked).State = EntityState.Detached;

            var controller = new CategoryController(context);

            var updated = new Category { Id = 30, Name = "NewName" };

            // Act
            var result = await controller.Edit(30, updated);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            // Confirm changes
            var category = context.Categories.First(c => c.Id == 30);
            Assert.Equal("NewName", category.Name);
        }

        [Fact]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 5, Name = "MismatchTest" });
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Act
            var result = await controller.Edit(999, new Category { Id = 5, Name = "Mismatch" });

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_Get_ExistingId_ReturnsView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 40, Name = "DeleteMe" });
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Act
            var result = await controller.Delete(40) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Category>(result.Model);
            Assert.Equal(40, model.Id);
            Assert.Equal("DeleteMe", model.Name);
        }

        [Fact]
        public async Task Delete_Get_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new CategoryController(context);

            // Act
            var result = await controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ExistingId_RemovesCategoryAndRedirects()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 50, Name = "DeleteMe2" });
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Act
            var result = await controller.DeleteConfirmed(50);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            // Confirm it is deleted
            Assert.False(context.Categories.Any(c => c.Id == 50));
        }
    }
}
