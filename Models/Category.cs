using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models;

public class Category
{
    public int Id { get; set; }

    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation property
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}