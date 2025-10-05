using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Lumen_Merch_Store.Models;

public class ApplicationUser : IdentityUser<int>
{
    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;

    [MaxLength(150)] public override string? PhoneNumber { get; set; }

    [MaxLength(255)] public string? PhotoUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}