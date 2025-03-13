namespace Dierentuin.Models;

public class Zoo
{
    // Primaire sleutel
    public int Id { get; set; }

    // Naam van de dierentuin
    [Required]
    public string Naam { get; set; } = "Mijn Dierentuin";

    // Alle verblijven in deze dierentuin
    public List<Enclosure> Verblijven { get; set; } = new();

    // Lijst met alledieren
    public List<Animal> AlleDieren { get; set; } = new();


    //Zonsopgang roep de Zonsopgang-methode aan voor elk verblijf dieren bepalen of ze opstaan  of slapen
    public void Zonsopgang()
    {
        foreach (var verblijf in Verblijven)
        {
            verblijf.Zonsopgang();
        }
    }

    // Zonsondergang roep de Zonsondergang-methode aan voor elk verblijf dieren bepalen of  ze opstaan of slapen
   
    public void Zonsondergang()
    {
        foreach (var verblijf in Verblijven)
        {
            verblijf.Zonsondergang();
        }
    }

    
    // Voedertijd: Roep de Voedertijd-methode aan voor elk verblijf 
    public void Voedertijd()
    {
        foreach (var verblijf in Verblijven)
        {
            verblijf.Voedertijd();
        }
    }


    // CheckConstraints: controleer voor elk verblijf
    // of aan de eisen is voldaan (ruimte, beveiliging, etc.)
    // Keer terug met een lijst van meldingen.
  
    // <returns>Lijst met foutmeldingen</returns>
    public List<string> CheckConstraints()
    {
        var fouten = new List<string>();

        foreach (var verblijf in Verblijven)
        {
            var verblijfFouten = verblijf.CheckConstraints();
            if (verblijfFouten.Any())
            {
                // Je kunt prefixen met de naam van het verblijf
                foreach (var fout in verblijfFouten)
                {
                    fouten.Add($"[{verblijf.Name}] {fout}");
                }
            }
        }

        return fouten;
    }

    // AutoAssign: automatisch dieren indelen in verblijven.
    // removeExisting = true => verwijder alle verblijven + indeling
    // en maak een nieuwe indeling.
    // removeExisting = false => gebruik bestaande verblijven en vul niet toegewezen dieren aan.
    public void AutoAssign(bool removeExisting = false)
    {
        if (removeExisting)
        {
            // Extreem voorbeeld: verwijder alle verblijven
            Verblijven.Clear();
            // Maak hier eventueel nieuwe verblijven aan:
            Verblijven.Add(new Enclosure
            {
                Name = "Nieuw Verblijf 1",
                Size = 100,
                SecurityLevel = SecurityLevel.Medium
            });
            Verblijven.Add(new Enclosure
            {
                Name = "Nieuw Verblijf 2",
                Size = 200,
                SecurityLevel = SecurityLevel.High
            });
        }

       
        var unassignedAnimals = AlleDieren.Where(a => a.EnclosureId == null).ToList();

        foreach (var dier in unassignedAnimals)
        {

            var suitableEnclosure = Verblijven.FirstOrDefault();
            if (suitableEnclosure != null)
            {
                suitableEnclosure.Animals.Add(dier);
                dier.EnclosureId = suitableEnclosure.Id;
            }
        }
        

    }
}
}