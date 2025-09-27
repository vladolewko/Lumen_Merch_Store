using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Ім'я")]
    [StringLength(100, ErrorMessage = "Ім'я не може бути довше 100 символів.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "Пароль має бути не менше {2} і не більше {1} символів.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження паролю")]
    [Compare("Password", ErrorMessage = "Пароль та його підтвердження не співпадають.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}