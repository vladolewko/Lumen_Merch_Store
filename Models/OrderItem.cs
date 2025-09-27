using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumen_Merch_Store.Models;

public class OrderItem
{
    public int Id { get; set; }
        
    [Required]
    public int OrderId { get; set; }
        
    [Required]
    public int ProductId { get; set; }
        
    public int? SizeId { get; set; }
        
    [Required]
    public int Quantity { get; set; }
        
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
        
    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductSize? Size { get; set; }
}