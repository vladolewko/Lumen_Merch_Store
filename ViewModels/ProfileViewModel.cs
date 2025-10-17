using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class ProfileViewModel
{
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Ім'я")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Телефон")]
    public string? Phone { get; set; }
    
    [Display(Name = "URL фото")]
    public string? PhotoUrl { get; set; }
    
    [Display(Name = "Дата реєстрації")]
    public DateTime CreatedAt { get; set; }
}