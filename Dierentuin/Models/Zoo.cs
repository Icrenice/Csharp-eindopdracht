using System.ComponentModel.DataAnnotations;
using Dierentuin.Models.Enums;

namespace Dierentuin.Models
{
    public class Zoo
    {
        // Primary Key
        public int Id { get; set; }

        // Zoo name
        [Required]
        public string Name { get; set; } = "My Zoo";

        // All enclosures in this zoo
        public List<Enclosure> Enclosures { get; set; } = new();

        // All animals in the zoo (regardless of enclosure)
        public List<Animal> AllAnimals { get; set; } = new();

        /// <summary>
        /// Sunrise: calls the Sunrise method in each enclosure
        /// (so animals wake up or go to sleep accordingly)
        /// </summary>
        public void Sunrise()
        {
            foreach (var enclosure in Enclosures)
            {
                enclosure.Sunrise();
            }
        }

        /// <summary>
        /// Sunset: calls the Sunset method in each enclosure
        /// (so animals wake up or go to sleep accordingly)
        /// </summary>
        public void Sunset()
        {
            foreach (var enclosure in Enclosures)
            {
                enclosure.Sunset();
            }
        }

        /// <summary>
        /// FeedingTime: calls the FeedingTime method in each enclosure.
        /// </summary>
        public void FeedingTime()
        {
            foreach (var enclosure in Enclosures)
            {
                enclosure.FeedingTime();
            }
        }

        /// <summary>
        /// Checks constraints in each enclosure (space, security, etc.).
        /// Returns a list of error messages.
        /// </summary>
        public List<string> CheckConstraints()
        {
            var errors = new List<string>();

            foreach (var enclosure in Enclosures)
            {
                var enclosureErrors = enclosure.CheckConstraints();
                if (enclosureErrors.Any())
                {
                    // Prefix each error with the enclosure name
                    foreach (var error in enclosureErrors)
                    {
                        errors.Add($"[{enclosure.Name}] {error}");
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// AutoAssign: automatically assigns animals to enclosures.
        /// removeExisting = true => remove all enclosures + existing assignments,
        /// then create a new assignment.
        /// removeExisting = false => keep existing enclosures and only assign unassigned animals.
        /// </summary>
        public void AutoAssign(bool removeExisting = false)
        {
            if (removeExisting)
            {
                // Extreme example: remove all enclosures
                Enclosures.Clear();

                // Optionally add new enclosures here:
                Enclosures.Add(new Enclosure
                {
                    Name = "New Enclosure 1",
                    Size = 100,
                    SecurityLevel = SecurityLevel.Medium
                });
                Enclosures.Add(new Enclosure
                {
                    Name = "New Enclosure 2",
                    Size = 200,
                    SecurityLevel = SecurityLevel.High
                });
            }

            // Find all animals without an assigned enclosure
            var unassignedAnimals = AllAnimals
                .Where(a => a.EnclosureId == null)
                .ToList();

            // For simplicity, just place them in the first enclosure that exists
            foreach (var animal in unassignedAnimals)
            {
                var suitableEnclosure = Enclosures.FirstOrDefault();
                if (suitableEnclosure != null)
                {
                    suitableEnclosure.Animals.Add(animal);
                    animal.EnclosureId = suitableEnclosure.Id;
                }
            }
        }
    }
}
