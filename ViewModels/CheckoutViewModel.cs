using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class CheckoutViewModel
{
    public CartViewModel Cart { get; set; } = new();
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Phone is required")]
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Delivery address is required")]
    [StringLength(500)]
    public string DeliveryAddress { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Notes { get; set; }
}

