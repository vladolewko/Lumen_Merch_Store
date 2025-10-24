using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumen_Merch_Store.Models;

public class Product
{
    public int Id { get; set; }

    [Required] public int UniverseId { get; set; }

    [Required] public int CategoryId { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; } = 0;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    public string? ImageUrl { get; set; } 

    // All navigation properties with virtual

    public virtual ICollection<ProductTranslation> Translations { get; set; } = new List<ProductTranslation>();
    public Universe Universe { get; set; } = null!;   
    public Category Category { get; set; } = null!;
    public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}