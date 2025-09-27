using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public int RoleId { get; set; } = 2; // Default role
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(150)]
        public override string? PhoneNumber { get; set; }
        
        [MaxLength(255)]
        public string? PhotoUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}