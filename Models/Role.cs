using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models;

public class Role
{
    public int Id { get; set; }
        
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
        
    // Navigation property
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}