using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Запам'ятати мене?")]
    public bool RememberMe { get; set; }
}