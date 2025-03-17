using System.ComponentModel.DataAnnotations;
using Dierentuin.Models.Enums;

namespace Dierentuin.Models
{
    public class Animal
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Species { get; set; } = string.Empty;

        // Category kan null zijn => Id is nullable
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public Size Size { get; set; }
        public DietaryClass DietaryClass { get; set; }
        public ActivityPattern ActivityPattern { get; set; }

        // Voor de logica "eten": 
        //   - In EF: m:n many-to-many, we configureren dit in AppDbContext
        public List<Animal> Prey { get; set; } = new List<Animal>();

        // Verblijf kan null zijn
        public int? EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }

        public double SpaceRequirement { get; set; }
        public SecurityLevel SecurityRequirement { get; set; }

        // Handig om te tracken of een dier wakker is
        public bool IsAwake { get; set; } = true;
    }
}