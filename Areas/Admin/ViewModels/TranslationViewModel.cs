using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lumen_Merch_Store.Areas.Admin.ViewModels;


public class TranslationViewModel
{
    public string LanguageCode { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Назва є обов'язковою")]
    public string Name { get; set; } = string.Empty;
        
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; } // Тільки для Products
    public string? Description { get; set; } // Для Categories/Universes
}