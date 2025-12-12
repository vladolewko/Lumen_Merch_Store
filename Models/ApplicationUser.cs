using System.ComponentModel.DataAnnotations;
using Lumen_Merch_Store.Resources; // Цей using залишається непотрібним для цього варіанту
using Microsoft.AspNetCore.Identity;
 
namespace Lumen_Merch_Store.Models;

public class ApplicationUser : IdentityUser<int>
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public override string Email { get; set; } 
    
    [RegularExpression(@"^\+?(\d[\d\s-]{7,})$")]
    [StringLength(150)]
    public override string? PhoneNumber { get; set; } 
     
    [DataType(DataType.Url)]
    [StringLength(255)]
    public string? PhotoUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}