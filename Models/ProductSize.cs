using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models;

public class ProductSize
{
    public int Id { get; set; }
        
    [Required]
    public int ProductId { get; set; }
        
    [Required]
    [MaxLength(50)]
    public string Size { get; set; } = string.Empty;
        
    public int Stock { get; set; } = 0;
        
    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}