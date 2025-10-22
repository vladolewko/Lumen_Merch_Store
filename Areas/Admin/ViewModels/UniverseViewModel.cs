// Lumen_Merch_Store.Areas.Admin.ViewModels/UniverseViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Areas.Admin.ViewModels;

public class UniverseViewModel
{
    public int Id { get; set; }

    // Поля перекладу (Українська)
    [Required(ErrorMessage = "Введіть назву всесвіту (Українською)")]
    [StringLength(100)]
    [Display(Name = "Назва (Українська)")]
    public string NameUk { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Опис (Українська)")]
    public string? DescriptionUk { get; set; }

    [Required(ErrorMessage = "Введіть назву всесвіту (Англійською)")]
    [StringLength(100)]
    [Display(Name = "Назва (Англійська)")]
    public string NameEn { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Опис (Англійська)")]
    public string? DescriptionEn { get; set; }
}