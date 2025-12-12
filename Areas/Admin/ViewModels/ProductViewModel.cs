using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lumen_Merch_Store.Areas.Admin.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }

    // === ЗАГАЛЬНІ ПОЛЯ (Спільні для всіх мов) ===
    [Required(ErrorMessage = "Ціна є обов'язковою.")]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int UniverseId { get; set; }
    
    // Фото
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
    public List<TranslationViewModel> Translations { get; set; } = new();

    public string? NameForGrid { get; set; }
    // Dropdowns
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Universes { get; set; }
}