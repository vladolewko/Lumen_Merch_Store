using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Models;

[Index(nameof(UserId), nameof(ProductId), IsUnique = true)]
public class Favorite
{
    public int Id { get; set; }

    [Required] public int UserId { get; set; }

    [Required] public int ProductId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}