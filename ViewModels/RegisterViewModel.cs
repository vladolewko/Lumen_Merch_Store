using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Name")]
    [StringLength(100, ErrorMessage = "Ім'я не може бути довше 100 символів.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "Password lenght must be between 6 and 100 symbols", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}