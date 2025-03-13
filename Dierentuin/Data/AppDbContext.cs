using Microsoft.EntityFrameworkCore;
using Dierentuin.Models;
using Dierentuin.Models.Enums;
using Bogus;

namespace Dierentuin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enclosure> Enclosures => Set<Enclosure>();
        public DbSet<Zoo> Zoos => Set<Zoo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many: Animal -> Prey
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.Prey)
                .WithMany();

            // 1 -> many: Zoo -> Enclosures
            // (staat impliciet in de navigatieproperty Zoo.Enclosures)

            // Seeding via Bogus:
            // Stap 1: Bestaande Zoo aanmaken
            var zoo = new Zoo
            {
                Id = 1,
                Name = "Bogus Dierentuin"
            };
            modelBuilder.Entity<Zoo>().HasData(zoo);

            // Stap 2: Genereer Categories
            var categoryNames = new[] { "Zoogdieren", "Vogels", "Reptielen", "Insecten", "Amfibieën" };
            var categories = new List<Category>();
            int catId = 1;
            foreach (var cn in categoryNames)
            {
                categories.Add(new Category
                {
                    Id = catId++,
                    Name = cn
                });
            }
            modelBuilder.Entity<Category>().HasData(categories);

            // Stap 3: Genereer Enclosures (verblijven)
            // Bijv. 4 stuks met random data
            var enclosureFaker = new Faker<Enclosure>()
                .RuleFor(e => e.Id, f => f.IndexFaker + 1)
                .RuleFor(e => e.Name, f => "Verblijf " + f.Commerce.Department())
                .RuleFor(e => e.Climate, f => f.PickRandom<Climate>())
                .RuleFor(e => e.HabitatType, f => f.PickRandom<HabitatType>())
                .RuleFor(e => e.SecurityLevel, f => f.PickRandom<SecurityLevel>())
                .RuleFor(e => e.Size, f => f.Random.Double(50, 500));

            var enclosuresList = enclosureFaker.Generate(4);
            // Let op: we moeten Id's uniek maken
            int encCounter = 1;
            foreach (var enc in enclosuresList)
            {
                enc.Id = encCounter++;
            }
            modelBuilder.Entity<Enclosure>().HasData(enclosuresList);

            // Stap 4: Genereer Animals
            //   - We pikken random CategoryId uit [1..5]
            //   - Random EnclosureId uit [1..4] (of null voor wat 'zwevende' dieren)
            var animalFaker = new Faker<Animal>()
                .RuleFor(a => a.Id, f => f.IndexFaker + 1)
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Species, f => f.Commerce.ProductName()) // pseudo, net even wat anders
                .RuleFor(a => a.CategoryId, f => f.Random.Bool(0.8f)
                    ? f.Random.Int(1, categories.Count)
                    : (int?)null) // 80% wel category
                .RuleFor(a => a.Size, f => f.PickRandom<Size>())
                .RuleFor(a => a.DietaryClass, f => f.PickRandom<DietaryClass>())
                .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
                .RuleFor(a => a.SpaceRequirement, f => f.Random.Double(1, 50))
                .RuleFor(a => a.SecurityRequirement, f => f.PickRandom<SecurityLevel>())
                .RuleFor(a => a.IsAwake, f => f.Random.Bool())
                .RuleFor(a => a.EnclosureId, f => f.Random.Bool(0.7f)
                    ? f.Random.Int(1, enclosuresList.Count)
                    : (int?)null);

            var animalsList = animalFaker.Generate(15); // 15 dieren
            // Fix Id's (EF seeding: alle IDs unique)
            int animalId = 1;
            foreach (var a in animalsList)
            {
                a.Id = animalId++;
            }
            modelBuilder.Entity<Animal>().HasData(animalsList);
        }
    }
}
