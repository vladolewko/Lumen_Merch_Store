using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lumen_Merch_Store.Areas.Admin.ViewModels;

public class ProductViewModel
{
    // Базові поля Product
    public int Id { get; set; }

    [Required(ErrorMessage = "Ціна є обов'язковою.")]
    [Range(0.01, 100000.00, ErrorMessage = "Ціна має бути більше нуля.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Запас є обов'язковим.")]
    [Range(0, int.MaxValue, ErrorMessage = "Запас не може бути від'ємним.")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Потрібно обрати категорію.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Потрібно обрати всесвіт.")]
    public int UniverseId { get; set; }

    // Поля для перекладу (припускаємо, що основна мова - українська 'uk')
    [Required(ErrorMessage = "Назва продукту українською є обов'язковою.")]
    [MaxLength(150)]
    public string NameUk { get; set; } = string.Empty;

    [MaxLength(255)]
    public string ShortDescriptionUk { get; set; } = string.Empty;

    [MaxLength(1024)]
    public string FullDescriptionUk { get; set; } = string.Empty;

    // Поля для відображення списків (Dropdowns)
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Universes { get; set; }
}