namespace Dierentuin.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    // Alle dieren in deze categorie
    public List<Animal> Animals { get; set; } = new();
}
}