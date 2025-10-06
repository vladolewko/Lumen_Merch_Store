using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models;

public class Category
{
    public int Id { get; set; }

    // Navigation property
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    
    public virtual ICollection<CategoryTranslation> Translations { get; set; } = new List<CategoryTranslation>();
}