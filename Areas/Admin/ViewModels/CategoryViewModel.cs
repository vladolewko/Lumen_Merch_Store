using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Areas.Admin.ViewModels;

// ViewModel для відображення, створення та редагування Категорій
public class CategoryViewModel
{
    public int Id { get; set; }

    // Поля перекладу (Українська)
    [Required(ErrorMessage = "Введіть назву категорії")]
    [StringLength(100)]
    [Display(Name = "Назва (Українська)")]
    public string NameUk { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Опис (Українська)")]
    public string? DescriptionUk { get; set; }
}