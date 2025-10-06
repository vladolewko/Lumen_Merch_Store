using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models;

public class Universe
{
    public int Id { get; set; }
    
    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
    
    public virtual ICollection<UniverseTranslation> Translations { get; set; } = new List<UniverseTranslation>();
}