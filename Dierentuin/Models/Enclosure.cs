

using System.ComponentModel.DataAnnotations;
using System.Linq;             
using System.Collections.Generic;
namespace Dierentuin.Models;

public class Enclosure
{
    // Primaire sleutel
    public int Id { get; set; }

    // Verplichte naam van het verblijf
    [Required]
    public string Name { get; set; } = string.Empty;

    // Klimaattype (bijv. Tropical, Temperate, Arctic)
    public Klimaat Klimaat { get; set; }

    // Type leefgebied (bijv. Bos, Water, Woestijn, Grasland).
    public LeefgebiedType LeefgebiedType { get; set; }

    // Beveiligingsniveau (Low, Medium, High) – evt. hernoemen naar "BeveiligingsNiveau"
    public SecurityLevel SecurityLevel { get; set; }

    // Oppervlakte in vierkante meters
    public double Size { get; set; }

    // Navigatie-eigenschap: lijst met dieren in dit verblijf
    public List<Animal> Animals { get; set; } = new();

    
    /// Methode om aan te geven welke dieren wakker worden of gaan slapen bij zonsopgang.
  
    public void Zonsopgang()
    {
        foreach (var animal in Animals)
        {
            switch (animal.ActiviteitsPatroon)
            {
                case ActiviteitsPatroon.Dagactief:
                    animal.IsWakker = true;
                    break;
                case ActiviteitsPatroon.Nachtdier:
                    animal.IsWakker = false;
                    break;
                case ActiviteitsPatroon.Wisselend:
                    animal.IsWakker = true;
                    break;
            }
        }
    }

    
    /// Methode om aan te geven welke dieren wakker worden of gaan slapen bij zonsondergang.

    public void Zonsondergang()
    {
        foreach (var animal in Animals)
        {
            switch (animal.ActiviteitsPatroon)
            {
                case ActiviteitsPatroon.Dagactief:
                    animal.IsWakker = false;
                    break;
                case ActiviteitsPatroon.Nachtdier:
                    animal.IsWakker = true;
                    break;
                case ActiviteitsPatroon.Wisselend:
                    animal.IsWakker = false;
                    break;
            }
        }
    }

   
    /// Methode om aan te geven welke dieren wat eten  .
   

    public void Voedertijd()
    {
        // Sorteer dieren van klein naar groot:
        var orderedAnimals = Animals.OrderBy(a => a.Size).ToList();

        for (int i = 0; i < orderedAnimals.Count; i++)
        {
            var carnivoor = orderedAnimals[i];
            if (carnivoor.DietaryClass == DietaryClass.carnivoor) 
            {
                // Vind dieren die kleiner zijn:
                var prooien = orderedAnimals
                    .Where(p => p.Size < carnivoor.Size)
                    .ToList();

                // Haal deze 'prooien' uit het verblijf
                foreach (var prooi in prooien)
                {
                    Animals.Remove(prooi);
                }
            }
           
        }
    }

    
    /// Methode om te controleren of alle voorwaarden in dit verblijf kloppen:
    /// - Is er voldoende ruimte?
    /// - Voldoet het beveiligingsniveau aan de eisen van elk dier?
  
    /// <returns>Lijst met foutmeldingen (indien er problemen zijn).</returns>
    public List<string> CheckConstraints()
    {
        var fouten = new List<string>();

        // Check totale benodigde ruimte:
        double totaleBenodigdeRuimte = Animals.Sum(a => a.SpaceRequirement);
        if (totaleBenodigdeRuimte > Size)
        {
            fouten.Add($"Te weinig ruimte in '{Name}': " +
                       $"Nodig is {totaleBenodigdeRuimte} m², maar beschikbaar is {Size} m².");
        }

        // Check beveiliging:
        foreach (var animal in Animals)
        {
            if (animal.SecurityRequirement > SecurityLevel)
            {
                fouten.Add($"Beveiligingsniveau te laag voor dier '{animal.Name}' " +
                           $"(Benodigd: {animal.SecurityRequirement}, Huidig: {SecurityLevel}).");
            }
        }

    
        return fouten;
    }
