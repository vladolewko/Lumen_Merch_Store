using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumen_Merch_Store.Models;

public class Product
{
    public int Id { get; set; }

    [Required] public int UniverseId { get; set; }

    [Required] public int CategoryId { get; set; }

    [Required] [MaxLength(150)] public string Name { get; set; } = string.Empty;

    [MaxLength(255)] public string? ShortDescription { get; set; }

    public string? FullDescription { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; } = 0;

    // Navigation properties
    public virtual Universe Universe { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}