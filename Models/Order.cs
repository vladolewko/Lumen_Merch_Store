using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lumen_Merch_Store.Models.Enums;

namespace Lumen_Merch_Store.Models;

public class Order
{
    public int Id { get; set; }

    [Required] public int UserId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}