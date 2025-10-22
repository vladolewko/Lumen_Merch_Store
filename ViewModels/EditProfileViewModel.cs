using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "Введіть ім'я")]
    [Display(Name = "Ім'я:")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Введіть email")]
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    [Display(Name = "Email:")]
    public string Email { get; set; } = string.Empty;
    
    [Phone(ErrorMessage = "Невірний формат телефону")]
    [Display(Name = "Телефон:")]
    public string? Phone { get; set; }
    
    [Display(Name = "Фото профілю:")]
    public string? PhotoUrl { get; set; }
    
    [Display(Name = "Нове фото:")]
    public IFormFile? PhotoFile { get; set; }
}
