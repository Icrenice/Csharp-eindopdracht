using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Dierentuin.Models.Enums; //

namespace Dierentuin.Models
{
    public class Enclosure
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Climate (e.g., Tropical, Temperate, Arctic)
        public Climate Climate { get; set; }

        // Habitat type (e.g., Forest, Aquatic, Desert, Grassland)
        public HabitatType HabitatType { get; set; }

        // Security level (Low, Medium, High)
        public SecurityLevel SecurityLevel { get; set; }

        // Area in square meters
        public double Size { get; set; }

        // List of animals in this enclosure
        public List<Animal> Animals { get; set; } = new();

        /// <summary>
        /// Indicates which animals wake up or go to sleep at sunrise.
        /// </summary>
        public void Sunrise()
        {
            foreach (var animal in Animals)
            {
                switch (animal.ActivityPattern)
                {
                    case ActivityPattern.Diurnal:
                        // Day-active => awake at sunrise
                        animal.IsAwake = true;
                        break;
                    case ActivityPattern.Nocturnal:
                        // Night-active => sleeps at sunrise
                        animal.IsAwake = false;
                        break;
                    case ActivityPattern.Cathemeral:
                        // Mixed => pick awake or do your own logic
                        animal.IsAwake = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Indicates which animals wake up or go to sleep at sunset.
        /// </summary>
        public void Sunset()
        {
            foreach (var animal in Animals)
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
        }

        /// <summary>
        /// Indicates feeding time. Carnivores eat any smaller animals in the same enclosure first.
        /// </summary>
        public void FeedingTime()
        {
            // Sort animals from smallest to largest
            var orderedAnimals = Animals.OrderBy(a => a.Size).ToList();

            for (int i = 0; i < orderedAnimals.Count; i++)
            {
                var predator = orderedAnimals[i];
                if (predator.DietaryClass == DietaryClass.Carnivore)
                {
                    // Find all animals that are smaller
                    var preyList = orderedAnimals
                        .Where(p => p.Size < predator.Size)
                        .ToList();

                    // Remove those prey animals from the enclosure
                    foreach (var prey in preyList)
                    {
                        Animals.Remove(prey);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if all constraints are met:
        /// - Enough space for all animals
        /// - Security level >= each animal's security requirement
        /// Returns a list of error messages if something is wrong.
        /// </summary>
        public List<string> CheckConstraints()
        {
            var errors = new List<string>();

            // Check total required space
            double totalRequiredSpace = Animals.Sum(a => a.SpaceRequirement);
            if (totalRequiredSpace > Size)
            {
                errors.Add($"Not enough space in '{Name}': needed {totalRequiredSpace} m², available {Size} m².");
            }

            // Check security
            foreach (var animal in Animals)
            {
                if (animal.SecurityRequirement > SecurityLevel)
                {
                    errors.Add(
                        $"Security level too low for animal '{animal.Name}' " +
                        $"(Required: {animal.SecurityRequirement}, Current: {SecurityLevel})."
                    );
                }
            }

            return errors;
        }
    }
}
